using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingSeb.ExpressionEvaluator;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SP.Core.Enum;
using SP.Core.Master;
using SP.Core.Model;
using SP.Data;
using SP.Service.DTO;
using SP.Service.Excel;
using SP.Service.Services;
using Npgsql;
using SP.Core.Log;

namespace SP.Service.Background
{
    public interface IBackgroundInventoryService
    {
        Task<bool> UploadAsync(Guid serviceKey, List<UploadedFile> files, string aspNetUserId);
        Task<bool> AutoMergeAsync(Guid serviceKey, string aspNetUserId);
        Task<bool> CalculateBalanceAsync(Guid serviceKey, string aspNetUserId, int[] regions, int[] terrs, int[] stations,
            int[] groups, int[] noms, int[] usefulLife);
        Task<bool> CalculateOrderAsync(Guid serviceKey, int[] idList, string aspNetUserId);
    }

    public class BackgroundInventoryService : IBackgroundInventoryService
    {
        private readonly IBackgroundCoordinator _coordinator;
        private readonly IExcelParser _parser;
        private readonly IMasterService _masterService;
        private readonly IGasStationService _stationService;
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public BackgroundInventoryService(IBackgroundCoordinator coordinator)
        {
            _coordinator = coordinator;
            _parser = new ExcelParser();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var options = optionsBuilder
                .UseNpgsql(coordinator.Options.DefaultConnection)
                .UseLoggerFactory(ApplicationDbContext.ApplicationDbLoggerFactory)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .Options;
            _context = new ApplicationDbContext(options);
            _masterService = new MasterService(_context);
            _stationService = new GasStationService(_context);
            _inventoryService = new InventoryService(coordinator, _context);
        }

        /// <summary>
        /// Загрузить ТМЦ из файлов
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<bool> UploadAsync(Guid serviceKey, List<UploadedFile> files, string aspNetUserId)
        {
            // регистрируем в координаторе
            var progressIndicator = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.Created,
                Progress = 0
            };

            _coordinator.AddOrUpdate(progressIndicator);
            StringBuilder processingLog = new StringBuilder();
            Stopwatch sw = new Stopwatch();

            try
            {
                var person = await _masterService.GetPersonAsync(aspNetUserId);

                int totalFiles = files.Count;
                int fileCount = 1;

                foreach (var file in files)
                {
                    string stepMessageTemplate = "Парсинг файла [" + fileCount + "/" + totalFiles + "]: \"{0}\", лист: \"{1}\"";
                    List<ParsedInventory> parsedData = null;
                    // парсинг
                    using (var p = new ExcelPackage(file.FileInfo))
                    {
                        foreach (var ws in p.Workbook.Worksheets)
                        {
                            var colDefs = GenerateColumnDefinitions();
                            if (!_parser.ProbeColumnHeaders(ws, colDefs, out int headerRow))
                            {
                                continue;
                            }

                            string stepMessage = string.Format(stepMessageTemplate, file.FileName, ws.Name);
                            var parsingProgress = new BackgroundServiceProgress
                            {
                                Key = serviceKey,
                                Status = BackgroundServiceStatus.Running,
                                Step = stepMessage,
                                Progress = 0,
                                Log = null
                            };

                            processingLog.AppendLine(stepMessage);

                            var wsColumns = colDefs
                                .Where(x => x.ColumnIndex != null)
                                .ToArray();

                            sw.Start();
                            int lastRow = ws.Dimension.End.Row;
                            parsedData = new List<ParsedInventory>(lastRow - headerRow);
                            // извлекаем данные из листа Excel и сохраняем в списке
                            for (int r = headerRow + 1; r <= lastRow; r++)
                            {
                                var parsedRowData = _parser.ParseDataRow(ws, wsColumns, r);
                                if (parsedRowData?.Count > 0)
                                {
                                    if (decimal.TryParse(parsedRowData["Quantity"], NumberStyles.Number,
                                        CultureInfo.CurrentCulture, out decimal qty))
                                    {
                                        var parsedInventory = new ParsedInventory
                                        {
                                            StationCodeSAP = parsedRowData["StationCodeSAP"],
                                            StationPetronicsName = parsedRowData["StationPetronicsName"],
                                            InventoryCode = parsedRowData["InventoryCode"],
                                            InventoryName = parsedRowData["InventoryName"],
                                            MeasureUnitName = parsedRowData["MeasureUnitName"],
                                            Quantity = qty
                                        };

                                        parsedData.Add(parsedInventory);
                                    }
                                }

                                if (r % 100 == 0)
                                {
                                    parsingProgress.Progress = 100.0m * r / lastRow;
                                    _coordinator.AddOrUpdate(parsingProgress);
                                }
                            }
                            sw.Stop();
                            Debug.WriteLine($"Парсинг Excel файла {sw.ElapsedMilliseconds} мс");

                            var stations = await _stationService.GetGasStationIdentificationListAsync();
                            var measureUnits = await _masterService.GetDictionaryListAsync<MeasureUnit>();
                            // стыковка справочника АЗС
                            sw.Restart();
                            var dataWithStation = parsedData
                                .GroupJoin(stations,
                                    d => d.StationCodeSAP,
                                    s => s.CodeSAP,
                                    (d, s) => new
                                    {
                                        Data = d,
                                        Station = s
                                    })
                                .SelectMany(
                                    x => x.Station.DefaultIfEmpty(),
                                    (x, y) => new ParsedInventory
                                    {
                                        GasStationId = y?.Id,
                                        StationCodeSAP = x.Data.StationCodeSAP,
                                        StationPetronicsName = x.Data.StationPetronicsName,
                                        InventoryCode = x.Data.InventoryCode,
                                        InventoryName = x.Data.InventoryName,
                                        MeasureUnitName = x.Data.MeasureUnitName,
                                        Quantity = x.Data.Quantity
                                    })
                                .ToArray();
                            sw.Stop();
                            Debug.WriteLine($"Объединение со справочником АЗС {sw.ElapsedMilliseconds} мс");

                            var emptyStations = dataWithStation
                                .Where(x => x.GasStationId == null)
                                .Select(x => new { x.StationCodeSAP, x.StationPetronicsName })
                                .Distinct()
                                .OrderBy(z => z.StationCodeSAP);

                            if (emptyStations.Any())
                            {
                                processingLog.AppendLine("Обнаружены АЗС, которые отсутствуют в справочнике:");
                                foreach (var st in emptyStations)
                                {
                                    processingLog.AppendLine($"Код: {st.StationCodeSAP}, Наименование: {st.StationPetronicsName}");
                                }

                                processingLog.AppendLine("Вышеуказанные АЗС пропущены. Добавьте их в справочник и выполните загрузку остатков заново.");
                            }

                            // стыковка справочника единиц измерения
                            sw.Restart();
                            var dataWithMeasureUnits = dataWithStation
                                .GroupJoin(measureUnits,
                                    d => d.MeasureUnitName,
                                    m => m.Name,
                                    (d, m) => new
                                    {
                                        Data = d,
                                        MeasureUnit = m
                                    })
                                .SelectMany(
                                    x => x.MeasureUnit.DefaultIfEmpty(),
                                    (x, y) => new ParsedInventory
                                    {
                                        GasStationId = x.Data.GasStationId,
                                        StationCodeSAP = x.Data.StationCodeSAP,
                                        StationPetronicsName = x.Data.StationPetronicsName,
                                        InventoryCode = x.Data.InventoryCode,
                                        InventoryName = x.Data.InventoryName,
                                        MeasureUnitId = y?.Id,
                                        MeasureUnitName = x.Data.MeasureUnitName,
                                        Quantity = x.Data.Quantity
                                    })
                                .ToArray();
                            sw.Stop();
                            Debug.WriteLine($"Объединение со справочником единиц измерения {sw.ElapsedMilliseconds} мс");

                            var emptyMeasureUnits = dataWithMeasureUnits
                                .Where(x => x.MeasureUnitId == null)
                                .Select(x => x.MeasureUnitName)
                                .Distinct();

                            if (emptyMeasureUnits.Any())
                            {
                                processingLog.AppendLine("Обнаружены единицы измерения, которые отсутствуют в справочнике:");
                                processingLog.AppendLine(string.Join(", ", emptyMeasureUnits));
                                processingLog.AppendLine("ТМЦ с вышеуказанным единицами измерения пропущены. Добавьте их в справочник и выполните загрузку остатков заново.");
                            }

                            // поиск дублей
                            var duplicates = dataWithMeasureUnits
                                .Where(x => x.GasStationId.HasValue && x.MeasureUnitId.HasValue)
                                .GroupBy(x => new { x.GasStationId, x.InventoryCode })
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key);
                            if (duplicates.Any())
                            {
                                processingLog.AppendLine("Обнаружены задублированные ТМЦ, будут обработаны только первые попавшиеся значения:");
                                foreach (var key in duplicates)
                                {
                                    var firstDuplicateRecord = dataWithMeasureUnits.FirstOrDefault(x => x.GasStationId == key.GasStationId && x.InventoryCode == key.InventoryCode);
                                    if (firstDuplicateRecord == null)
                                        continue;

                                    processingLog.AppendLine($"Объект {firstDuplicateRecord.StationCodeSAP}, ТМЦ {firstDuplicateRecord.InventoryCode}, {firstDuplicateRecord.InventoryName}");
                                }
                            }

                            var inventoryData = dataWithMeasureUnits
                                .Where(x => x.GasStationId.HasValue && x.MeasureUnitId.HasValue)
                                .GroupBy(x => new { x.GasStationId, x.InventoryCode },
                                    (key, g) => g.FirstOrDefault()
                                )
                                .Select(z => new StageInventory
                                {
                                    Code = z.InventoryCode,
                                    Name = z.InventoryName,
                                    GasStationId = z.GasStationId.Value,
                                    MeasureUnitId = z.MeasureUnitId.Value,
                                    Quantity = z.Quantity,
                                    LastUpdate = DateTime.Now,
                                    PersonId = person.Id
                                })
                                .ToArray();

                            if (inventoryData.Any())
                            {
                                // пакетное сохранение в БД
                                processingLog.AppendLine("Выполняем сохранение остатков ТМЦ.");
                                sw.Restart();
                                await _inventoryService.PurgeStageInventoryAsync(person.Id);
                                await _inventoryService.SaveStageInventoryAsync(inventoryData, serviceKey, person.Id);
                                sw.Stop();
                                Debug.WriteLine($"Сохранение загруженных записей {sw.ElapsedMilliseconds} мс");
                                processingLog.AppendLine($"Сохранено {inventoryData.Length} записей.");
                            }
                            else
                            {
                                processingLog.AppendLine("Отсутствуют данные для загрузки остатков ТМЦ.");
                            }
                        }
                    }

                    fileCount++;
                }

                var successFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.RanToCompletion,
                    Step = "Загрузка завершена",
                    Progress = 100,
                    Log = processingLog.ToString()
                };

                _coordinator.AddOrUpdate(successFinalProgress);
            }
            catch (Exception ex)
            {
                processingLog.AppendLine(ex.Message);
                processingLog.AppendLine("Ошибка загрузки остатков ТМЦ. Проверьте лог приложения.");

                var badFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.Faulted,
                    Step = "Ошибка выполнения",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(badFinalProgress);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Сформировать список колонок для распознавания в Excel файле
        /// </summary>
        /// <returns></returns>
        private List<ColumnDefinition> GenerateColumnDefinitions()
        {
            var list = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "StationCodeSAP",
                    Title = "Склад SAP",
                    TitleComparisonMode = ComparisonMode.Equals,
                    DefaultIndex = 1
                },
                new ColumnDefinition
                {
                    Name = "StationPetronicsName",
                    Title = "Склад Петроникс",
                    TitleComparisonMode = ComparisonMode.StartsWith,
                    DefaultIndex = 2
                },
                new ColumnDefinition
                {
                    Name = "InventoryCode",
                    Title = "Код ТМЦ",
                    TitleComparisonMode = ComparisonMode.Equals,
                    DefaultIndex = 5
                },
                new ColumnDefinition
                {
                    Name = "InventoryName",
                    Title = "ТМЦ",
                    TitleComparisonMode = ComparisonMode.Equals,
                    DefaultIndex = 6
                },
                new ColumnDefinition
                {
                    Name = "MeasureUnitName",
                    Title = "Единица измерения",
                    TitleComparisonMode = ComparisonMode.Equals,
                    DefaultIndex = 7
                },
                new ColumnDefinition
                {
                    Name = "Quantity",
                    Title = "Кол-во",
                    TitleComparisonMode = ComparisonMode.Equals,
                    DefaultIndex = 11
                }
            };

            return list;
        }

        /// <summary>
        ///  Выполнить автоматическое объединение ТМЦ с Номенклатурой
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="aspNetUserId"></param>
        /// <returns></returns>
        public async Task<bool> AutoMergeAsync(Guid serviceKey, string aspNetUserId)
        {
            // регистрируем в координаторе
            var progressIndicator = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.Created,
                Step = "Автообъединение ТМЦ с Номенклатурой",
                Progress = 0
            };

            _coordinator.AddOrUpdate(progressIndicator);
            StringBuilder processingLog = new StringBuilder();
            Stopwatch sw = new Stopwatch();

            try
            {
                var person = await _masterService.GetPersonAsync(aspNetUserId);

                sw.Start();
                var totalRows = await _context.Inventories
                    .Where(x => !x.IsBlocked && !x.NomenclatureId.HasValue)
                    .CountAsync();
                sw.Stop();
                Debug.WriteLine($"Найдены неприкрепленные ТМЦ за {sw.ElapsedMilliseconds} мс");

                processingLog.AppendLine($"Найдено {totalRows} записей для автоматического объединения.");

                if (totalRows == 0)
                {
                    processingLog.AppendLine("Автообъединение завершено.");
                    var successNothingProgress = new BackgroundServiceProgress
                    {
                        Key = serviceKey,
                        Status = BackgroundServiceStatus.RanToCompletion,
                        Step = "Автообъединение завершено",
                        Progress = 100,
                        Log = processingLog.ToString()
                    };
                    _coordinator.AddOrUpdate(successNothingProgress);

                    return true;
                }

                var p1 = new NpgsqlParameter("person_id", person.Id);
                var p2 = new NpgsqlParameter("total_rows", DbType.Int32)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = 0
                };
                sw.Restart();
                await _context.Database.ExecuteSqlRawAsync("CALL \"AutoLinkInventoryWithNomenclature\"(@person_id, @total_rows);", p1, p2);
                sw.Stop();
                Debug.WriteLine($"Автоматическое объединение ТМЦ {sw.ElapsedMilliseconds} мс");


                processingLog.AppendLine($"Обработано {totalRows} записей ТМЦ.");
                processingLog.AppendLine($"Состыковано {p2.Value} записей ТМЦ с Номенклатурой.");

                var successFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.RanToCompletion,
                    Step = "Загрузка завершена",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(successFinalProgress);
            }
            catch (Exception ex)
            {
                processingLog.AppendLine(ex.Message);
                processingLog.AppendLine("Ошибка автоматического объединения ТМЦ. Проверьте лог приложения.");

                var badFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.Faulted,
                    Step = "Ошибка выполнения",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(badFinalProgress);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Рассчитать остатки по Номенклатуре (в разрезе АЗС и номенклатуры)
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="aspNetUserId"></param>
        /// <returns></returns> 
        public async Task<bool> CalculateBalanceAsync(Guid serviceKey, string aspNetUserId, int[] regions, int[] terrs, int[] stations,
            int[] groups, int[] noms, int[] usefulLife)
        {
            // регистрируем в координаторе
            var progressIndicator = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.Created,
                Progress = 0
            };

            _coordinator.AddOrUpdate(progressIndicator);
            StringBuilder processingLog = new StringBuilder();
            Stopwatch sw = new Stopwatch();

            try
            {
                processingLog.AppendLine("Расчет остатков Номенклатуры начат.");

                var person = await _masterService.GetPersonAsync(aspNetUserId);
                sw.Start();

                // формируем список id всей выбранной номенклатуры
                var nomenclatureQuery = _context.Nomenclatures.AsNoTracking();
                if (noms != null)
                {
                    nomenclatureQuery = nomenclatureQuery.Where(x => noms.Contains(x.Id));
                }
                else if (groups != null)
                {
                    nomenclatureQuery = nomenclatureQuery.Where(x => groups.Contains(x.NomenclatureGroupId));
                }

                if (usefulLife != null)
                {
                    foreach (var r in usefulLife)
                    {
                        switch ((UsefulLifeRange)r)
                        {
                            case UsefulLifeRange.LessThanYear:
                                nomenclatureQuery = nomenclatureQuery.Where(x => x.UsefulLife < 12);
                                break;
                            case UsefulLifeRange.Year:
                                nomenclatureQuery = nomenclatureQuery.Where(x => x.UsefulLife == 12);
                                break;
                            case UsefulLifeRange.GreaterThanYear:
                                nomenclatureQuery = nomenclatureQuery.Where(x => x.UsefulLife > 12);
                                break;
                        }
                    }
                }

                var nomenclatureIdList = await nomenclatureQuery
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToArrayAsync();
                int totalNomenclatures = nomenclatureIdList.Length;
                processingLog.AppendLine($"Количество выбранной Номенклатуры: {totalNomenclatures}");
                sw.Stop();
                Debug.WriteLine($"Номенклатура выбрана за {sw.ElapsedMilliseconds} мс");

                sw.Restart();
                // формируем список id все выбранных АЗС
                var stationQuery = _context.GasStations.AsNoTracking();
                if (stations != null)
                {
                    stationQuery = stationQuery.Where(x => stations.Contains(x.Id));
                }
                else if (terrs?.Length > 0)
                {
                    stationQuery = stationQuery.Where(x => terrs.Contains(x.TerritoryId.Value));
                }
                else if (regions?.Length > 0)
                {
                    stationQuery = stationQuery
                        .Include(x => x.Territory)
                        .Where(x => x.Territory.ParentId != null && regions.Contains(x.Territory.ParentId.Value));
                }

                var stationList = await stationQuery
                    .Select(x => new
                    {
                        x.Id,
                        x.StationNumber
                    })
                    .ToArrayAsync();
                int totalStations = stationList.Length;
                processingLog.AppendLine($"Количество выбранных АЗС: {totalStations}");
                sw.Stop();
                Debug.WriteLine($"АЗС выбраны за {sw.ElapsedMilliseconds} мс");

                // кол-в остатков для расчета = кол-во номенклатуры * кол-во АЗС
                int totalRows = totalNomenclatures * totalStations;
                int currentRow = 0;

                progressIndicator.Status = BackgroundServiceStatus.Running;

                sw.Start();

                while (true)
                {
                    var portion = stationList
                        .Skip(currentRow)
                        .Take(50)
                        .ToArray();
                    if (portion.Length == 0)
                    {
                        break;
                    }

                    var pStations = new NpgsqlParameter("stations", string.Join(',', stationList.Select(x => x.Id)));
                    var pNomenclatures = new NpgsqlParameter("nomenclatures", string.Join(',', nomenclatureIdList));
                    var pPerson = new NpgsqlParameter("person_id", person.Id);
                    var pRows = new NpgsqlParameter("total_rows", DbType.Int32)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = 0
                    };
                    sw.Restart();
                    await _context.Database.ExecuteSqlRawAsync("CALL \"CalculateBalance\"(@stations, @nomenclatures, @person_id, @total_rows);",
                        pStations, pNomenclatures, pPerson, pRows);

                    currentRow += totalNomenclatures;
                    progressIndicator.Progress = 100.0m * currentRow / totalRows;
                    _coordinator.AddOrUpdate(progressIndicator);
                }

                sw.Stop();
                Debug.WriteLine($"Остатки рассчитаны за {sw.ElapsedMilliseconds} мс");

                processingLog.AppendLine($"Расчет остатков Номенклатуры завершен");
                var successFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.RanToCompletion,
                    Step = "Расчет остатков завершен",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(successFinalProgress);
            }
            catch (Exception ex)
            {
                processingLog.AppendLine(ex.Message);
                processingLog.AppendLine("Ошибка расчета остатков ТМЦ. Проверьте лог приложения.");

                var badFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.Faulted,
                    Step = "Ошибка выполнения",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(badFinalProgress);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Рассчитать остатки и потребность в заказе Номенклатуры
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="idList"></param>
        /// <param name="aspNetUserId"></param>
        /// <returns></returns>
        public async Task<bool> CalculateOrderAsync(Guid serviceKey, int[] idList, string aspNetUserId)
        {
            // регистрируем в координаторе
            var progressIndicator = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.Created,
                Progress = 0
            };

            _coordinator.AddOrUpdate(progressIndicator);
            StringBuilder processingLog = new StringBuilder();

            try
            {
                processingLog.AppendLine("Расчет остатков и потребности в заказе ТМЦ начат.");

                var person = await _masterService.GetPersonAsync(aspNetUserId);

                int currentRow = 0;
                int totalRows = idList.Length;

                progressIndicator.Status = BackgroundServiceStatus.Running;
                _coordinator.AddOrUpdate(progressIndicator);

                while (true)
                {
                    var portion = idList
                        .Skip(currentRow)
                        .Take(100)
                        .ToArray();
                    if (portion.Length == 0)
                    {
                        break;
                    }

                    await CalcBalanceListAsync(portion, person.Id);

                    foreach (var id in portion)
                    {
                        await CalculateSingleOrder(id, person.Id);
                    }

                    await _context.SaveChangesAsync();

                    currentRow += 100;
                    progressIndicator.Progress = 100.0m * currentRow / totalRows;
                    _coordinator.AddOrUpdate(progressIndicator);
                }

                processingLog.AppendLine($"Расчет остатков и потребности в заказе ТМЦ завершен");
                var successFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.RanToCompletion,
                    Step = "Расчет остатков и потребности в заказе ТМЦ завершен",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(successFinalProgress);
            }
            catch (Exception ex)
            {
                processingLog.AppendLine(ex.Message);
                processingLog.AppendLine("Ошибка расчета потребности в заказе ТМЦ. Проверьте лог приложения.");

                var badFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.Faulted,
                    Step = "Ошибка выполнения",
                    Progress = 100,
                    Log = processingLog.ToString()
                };
                _coordinator.AddOrUpdate(badFinalProgress);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Рассчитать остатки по выбранной номенклатуре
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        private async Task<bool> CalcBalanceListAsync(int[] idList, int personId)
        {
            var pIdList = new NpgsqlParameter("id_list", string.Join(',', idList));
            var pPerson = new NpgsqlParameter("person_id", personId);
            var pRows = new NpgsqlParameter("total_rows", DbType.Int32)
            {
                Direction = ParameterDirection.InputOutput,
                Value = 0
            };
            await _context.Database.ExecuteSqlRawAsync("CALL \"CalcBalanceList\"(@id_list, @person_id, @total_rows);",
                pIdList, pPerson, pRows);

            return true;
        }

        /// <summary>
        /// Рассчитать потребность в заказе отдельной позиции Номенклатуры по АЗС
        /// </summary>
        /// <param name="id"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        private async Task<bool> CalculateSingleOrder(int id, int personId)
        {
            var nomBalance = await _context.CalcSheets.FirstOrDefaultAsync(x => x.Id == id);
            if (nomBalance == null)
            {
                return false;
            }

            decimal plan = 0.0m;
            if (nomBalance.FixedAmount != null)
            {
                plan = nomBalance.FixedAmount.Value;
            }
            else if (!string.IsNullOrWhiteSpace(nomBalance.Formula))
            {
                // параметры АЗС
                var gasStation = await _context.GasStations
                    .Include(x => x.Segment)
                    .Include(x => x.ServiceLevel)
                    .Include(x => x.OperatorRoomFormat)
                    .Include(x => x.TradingHallOperatingMode)
                    .Include(x => x.CashboxLocation)
                    .Include(x => x.TradingHallSize)
                    .Include(x => x.CashRegisterTape)
                    .FirstOrDefaultAsync(x => x.Id == nomBalance.GasStationId);

                var evaluator = new ExpressionEvaluator()
                {
                    OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = true
                };

                evaluator.Variables = GetEvaluatorVariables(gasStation);

                try
                {
                    var result = evaluator.Evaluate(nomBalance.Formula.ToLower());
                    if (!decimal.TryParse(result.ToString(), out plan))
                    {
                        throw new ArithmeticException($"Некорректная формула: {nomBalance.Formula}");
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                // округление
                switch(nomBalance.Rounding)
                {
                    case Rounding.Floor:
                        plan = Math.Floor(plan);
                        break;
                    case Rounding.Ceiling:
                        plan = Math.Ceiling(plan);
                        break;
                    case Rounding.Round:
                        plan = Math.Round(plan);
                        break;
                }
            }

            if (nomBalance.Plan == plan)
            {
                return true;
            }

            DateTime now = DateTime.Now;
            decimal oldPlan = nomBalance.Plan;
            nomBalance.Plan = plan;
            nomBalance.LastUpdate = now;
            _context.Entry(nomBalance).Property(r => r.Plan).IsModified = true;
            _context.Entry(nomBalance).Property(r => r.LastUpdate).IsModified = true;

            var historyRecord = CalcSheetHistory.CreateHistoryRecord(nomBalance, now);
            await _context.CalcSheetHistories.AddAsync(historyRecord);

            var changeLogRecord = new ChangeLog
            {
                PersonId = personId,
                ChangeDate = now,
                EntityName = "CalcSheet",
                ActionName = "CalcPlan",
                RecordId = nomBalance.Id,
                OldValue = $"План: {oldPlan}",
                NewValue = $"План: {plan}"
            };
            await _context.ChangeLogs.AddAsync(changeLogRecord);

            return true;
        }

        private Dictionary<string, object> GetEvaluatorVariables(GasStation gasStation)
        {
            var variables = new Dictionary<string, object>()
            {
                { "if", new Func<bool,double,double,double>((c, x, y) => c ? x : y)},
                { "Segment", gasStation.Segment?.Name.ToLower() },
                { "Cluster", gasStation.ServiceLevel?.Name.ToLower() },
                { "OperFormat", gasStation.OperatorRoomFormat?.Name.ToLower() },
                { "Regime", gasStation.TradingHallOperatingMode?.Name.ToLower() },
                { "Cashdesk", gasStation.CashboxLocation?.Name.ToLower() },
                { "SaleArea", Convert.ToDouble(gasStation.TradingHallArea) },
                { "TotalArm", gasStation.CashboxTotal },
                { "ManagerArm", gasStation.ManagerArmTotal },
                { "TotalTrk", gasStation.FuelDispenserTotal },
                { "TotalTrkPost", gasStation.FuelDispenserPostTotal },
                { "TotalTrkOpenPost", gasStation.FuelDispenserPostWithoutShedTotal },
                { "CashTape", gasStation.CashRegisterTape?.Name.ToLower() },
                { "AvgCheck", Convert.ToDouble(gasStation.ChequePerDay) },
                { "TotalClientToiletRoom", gasStation.ClientRestroomTotal },
                { "TotalTambour", gasStation.ClientTambourTotal },
                { "TotalSink", gasStation.ClientSinkTotal },
                { "Tambour", gasStation.HasJointRestroomEntrance },
                { "MonthIncome", Convert.ToDouble(gasStation.RevenueAvg) },
                { "Sibilla", gasStation.HasSibilla },
                { "Bakery", gasStation.HasBakery },
                { "Cakes", gasStation.HasCakes },
                { "Fries", gasStation.DeepFryTotal },
                { "Marmite", gasStation.HasMarmite },
                { "Kitchen", gasStation.HasKitchen },
                { "CoffeeMachine", gasStation.CoffeeMachineTotal },
                // TODO: исправить название в программе и БД
                { "TotalPersonal", Convert.ToDouble(gasStation.PersonnelPerDay) },
                { "AvgBandLength", Convert.ToDouble(gasStation.ChequeBandLengthPerDay) },
                { "ImageCoef", Convert.ToDouble(gasStation.RepresentativenessFactor) },
                { "ImageCoef3q", Convert.ToDouble(gasStation.RepresentativenessFactor3Quarter) },
                { "DayCleaning", gasStation.DayCleaningTotal },
                { "DayRefueling", gasStation.DayRefuelingTotal },
                { "Merrychef", gasStation.MerrychefTotal },
                { "Revenue", Convert.ToDouble(gasStation.RevenueAvg) },
                { "FuelCard", gasStation.HasFuelCardProgram },
                { "PersonnelTotal", gasStation.PersonnelTotal },
                { "FuelTracks", gasStation.FuelTrackPerYear },
                { "RailwayTanks", gasStation.RailwayTankPerYear },
                { "ShiftsPerDay", gasStation.ShiftPerDay },
                { "PersonnelPerShift", gasStation.PersonnelPerShift },
                { "WorkingPlaces", gasStation.WorkingPlaceTotal },
                { "DiningRooms", gasStation.DiningRoomTotal },
                { "Fuel92PerYear", Convert.ToDouble(gasStation.Fuel92PerYear) },
                { "Fuel95PerYear", Convert.ToDouble(gasStation.Fuel95PerYear) },
                { "Fuel100PerYear", Convert.ToDouble(gasStation.Fuel100PerYear) },
                { "DieselFuelPerYear", Convert.ToDouble(gasStation.DieselFuelPerYear) },
                { "FuelBaseAutomation", gasStation.HasFuelBaseAutomation },
                { "RailwayDeliveryPlan", Convert.ToDouble(gasStation.RailwayDeliveryPlanTotal) },
                { "Reservoirs", gasStation.ReservoirTotal },
                { "AntiIcingSquare", Convert.ToDouble(gasStation.AntiIcingSquare) },
                { "AntiIcingPerYear", gasStation.AntiIcingPerYear },
                { "RailwayFuelTanks", gasStation.RailwayTankPerYear },
                { "Departments", gasStation.DepartmentTotal },
                { "CentralWaterSupply", gasStation.HasCentralWaterSupply },
                { "LabTestsPerMonth", gasStation.AverageTestPerMonth },
                { "ServicingGasStations", gasStation.ServicingGasStationTotal },
                { "WorkingRooms", gasStation.WorkingRoomTotal },
                { "Sindy", gasStation.HasSindyAnalyzer },
                { "Spectroscan", gasStation.HasSpectroscan },
                { "ElectroLabSpecialists", gasStation.SpecialistTotalForElectricalTest},
                { "ElectroTestsPerYear", gasStation.ElectricalTestPerYear }
            };

            var lowered = variables.Select(x => new
            {
                Key = x.Key.ToLower(),
                x.Value
            }).ToDictionary(x => x.Key, x => x.Value);

            return lowered;
        }
    }
}
