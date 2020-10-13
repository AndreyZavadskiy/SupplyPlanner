using System;
using System.Collections.Generic;
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

namespace SP.Service.Background
{
    public interface IBackgroundInventoryService
    {
        Task<bool> UploadAsync(Guid serviceKey, List<UploadedFile> files, string aspNetUserId);
        Task<bool> AutoMergeAsync(Guid serviceKey, string aspNetUserId);
        Task<bool> CalculateBalanceAsync(Guid serviceKey, string aspNetUserId, int[] regions, int[] terrs, int? stationId,
            int? groupId, int? nomId, int[] usefulLife);
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
                .UseSqlServer(coordinator.Options.DefaultConnection)
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
                    string stepMessageTemplate =
                        "Парсинг файла [" + fileCount + "/" + totalFiles + "]: \"{0}\", лист: \"{1}\"";
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
                                    processingLog.AppendLine(
                                        $"Код: {st.StationCodeSAP}, Наименование: {st.StationPetronicsName}");
                                }

                                processingLog.AppendLine(
                                    "Вышеуказанные АЗС пропущены. Добавьте их в справочник и выполните загрузку остатков заново.");
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

                            var inventoryData = dataWithMeasureUnits
                                .Where(x => x.GasStationId.HasValue && x.MeasureUnitId.HasValue)
                                .Select(x => new StageInventory
                                {
                                    Code = x.InventoryCode,
                                    Name = x.InventoryName,
                                    GasStationId = x.GasStationId.Value,
                                    MeasureUnitId = x.MeasureUnitId.Value,
                                    Quantity = x.Quantity,
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

            try
            {
                var person = await _masterService.GetPersonAsync(aspNetUserId);

                var data = await _context.Inventories
                    .Where(x => !x.IsBlocked && !x.NomenclatureId.HasValue)
                    .ToArrayAsync();
                int totalRows = data.Length;

                processingLog.AppendLine($"Найдено {totalRows} записей для автоматического объединения.");

                if (totalRows == 0)
                {
                    processingLog.AppendLine("Выполнение завершено.");
                    var successNothingProgress = new BackgroundServiceProgress
                    {
                        Key = serviceKey,
                        Status = BackgroundServiceStatus.RanToCompletion,
                        Step = "Загрузка завершена",
                        Progress = 100,
                        Log = processingLog.ToString()
                    };
                    _coordinator.AddOrUpdate(successNothingProgress);

                    return true;
                }

                int currentRow = 0,
                    linkedRows = 0;
                while (true)
                {
                    var portion = data
                        .Skip(currentRow)
                        .Take(100)
                        .ToArray();
                    if (portion.Length == 0)
                    {
                        break;
                    }

                    // проверка по коду и наименованию в Номенклатуре
                    var linkedInventories = _context.Nomenclatures.AsEnumerable()
                        .Join(portion,
                            n => n.PetronicsCode,
                            p => p.Code,
                            (n, p) => new
                            {
                                Inventory = p,
                                NomenclatureId = n.Id,
                                PetronicsName = n.PetronicsName
                            }
                        )
                        .Where(z => z.Inventory.Name == z.PetronicsName)
                        .ToArray();

                    foreach (var rec in linkedInventories)
                    {
                        var dbRecord = rec.Inventory;
                        dbRecord.NomenclatureId = rec.NomenclatureId;
                        _context.Inventories.Attach(dbRecord);
                        _context.Entry(dbRecord).Property(r => r.NomenclatureId).IsModified = true;
                    }

                    // TODO проверка по коду Петроникса в Номенклатуре
                    // TODO проверка по наименованию в Номенклатуре
                    // TODO проверка по коду и наименованию в ТМЦ, если есть аналогичная привязка и наименования в ТМЦ и Номенклатуре различаются

                    await _context.SaveChangesAsync();

                    linkedRows += linkedInventories.Length;

                    currentRow += 100;
                    progressIndicator.Progress = currentRow / totalRows;
                    _coordinator.AddOrUpdate(progressIndicator);
                }

                processingLog.AppendLine($"Обработано {totalRows} записей ТМЦ.");
                processingLog.AppendLine($"Состыковано {linkedRows} записей ТМЦ с Номенклатурой.");

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
        /// Рассчитать остатки по Номенклатуре
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="aspNetUserId"></param>
        /// <returns></returns>
        public async Task<bool> CalculateBalanceAsync(Guid serviceKey, string aspNetUserId, int[] regions, int[] terrs, int? stationId,
            int? groupId, int? nomId, int[] usefulLife)
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
                processingLog.AppendLine("Расчет остатков Номенклатуры начат.");

                var person = await _masterService.GetPersonAsync(aspNetUserId);
                var nomenclatureQuery = _context.Nomenclatures.AsNoTracking();
                if (nomId != null)
                {
                    nomenclatureQuery = nomenclatureQuery.Where(x => x.Id == nomId);
                }
                else if (groupId != null)
                {
                    nomenclatureQuery = nomenclatureQuery.Where(x => x.NomenclatureGroupId == groupId);
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

                // список id всей активной номенклатуры
                var nomenclatureIdList = await nomenclatureQuery
                    .Where(x => x.IsActive)
                    .Select(x => x.Id)
                    .ToArrayAsync();
                int totalNomenclatures = nomenclatureIdList.Length;
                processingLog.AppendLine($"Количество Номенклатуры: {totalNomenclatures}");

                var stationQuery = _context.GasStations.AsNoTracking();
                if (stationId != null)
                {
                    stationQuery = stationQuery.Where(x => x.Id == stationId);
                }
                else if (terrs.Length > 0)
                {
                    stationQuery = stationQuery.Where(x => terrs.Contains(x.TerritoryId));
                }
                else if (regions.Length > 0)
                {
                    stationQuery = stationQuery
                        .Include(x => x.Territory)
                        .Where(x => x.Territory.ParentId != null && regions.Contains(x.Territory.ParentId.Value));
                }

                var stations = await stationQuery
                    .Select(x => new
                    {
                        x.Id,
                        x.StationNumber
                    })
                    .ToArrayAsync();
                int totalStations = stations.Length;
                processingLog.AppendLine($"Количество АЗС: {totalStations}");

                int currentStation = 1;
                // кол-в остатков для расчета = кол-во номенклатуры * кол-во АЗС
                int totalRows = totalNomenclatures * totalStations;
                int currentRow = 0;

                progressIndicator.Status = BackgroundServiceStatus.Running;

                foreach (var station in stations)
                {
                    progressIndicator.Step = $"АЗС {station.StationNumber} : {currentStation} из {totalStations}";
                    _coordinator.AddOrUpdate(progressIndicator);
                    processingLog.AppendLine($"АЗС {station.StationNumber}");

                    // предыдущие остатки
                    var oldBalances = await _context.CalcSheets.AsNoTracking()
                        .Where(x => x.GasStationId == station.Id)
                        .ToArrayAsync();
                    // остатки по АЗС
                    var inventoryBalances = await _context.Inventories.AsNoTracking()
                        .Where(x => x.GasStationId == station.Id && x.NomenclatureId.HasValue && !x.IsBlocked)
                        .GroupBy(x => x.NomenclatureId)
                        .Select(x => new
                        {
                            NomenclatureId = x.Key,
                            Quantity = x.Sum(i => i.Quantity)
                        })
                        .ToArrayAsync();
                    // обновляем существующие остатки
                    var existingBalances = oldBalances
                        .Join(inventoryBalances,
                            o => o.NomenclatureId,
                            b => b.NomenclatureId,
                            (o, b) => new
                            {
                                OldRecord = o,
                                NewQuantity = b.Quantity
                            })
                        .Where(z => z.OldRecord.Quantity != z.NewQuantity)
                        .ToArray();
                    foreach (var rec in existingBalances)
                    {
                        DateTime now = DateTime.Now;
                        var dbBalance = new CalcSheet
                        {
                            Id = rec.OldRecord.Id,
                            Quantity = rec.NewQuantity,
                            LastUpdate = now
                        };
                        _context.CalcSheets.Attach(dbBalance);
                        _context.Entry(dbBalance).Property(r => r.Quantity).IsModified = true;
                        _context.Entry(dbBalance).Property(r => r.LastUpdate).IsModified = true;

                        var historyRecord = CalcSheetHistory.CreateHistoryRecord(rec.OldRecord, now);
                        await _context.CalcSheetHistories.AddAsync(historyRecord);
                    }

                    await _context.SaveChangesAsync();
                    processingLog.AppendLine($"Обновлено записей: {existingBalances.Length}");

                    // добавляем новые остатки
                    var newNomenclatures = nomenclatureIdList
                        .Except(
                            _context.CalcSheets
                                .Where(x => x.GasStationId == station.Id)
                                .Select(x => x.NomenclatureId)
                            )
                        .ToArray();
                    var newBalances = newNomenclatures
                        .GroupJoin(inventoryBalances,
                            n => n,
                            i => i.NomenclatureId,
                            (n, i) => new
                            {
                                NomenclatureId = n,
                                Balance = i
                            })
                        .SelectMany(
                            x => x.Balance.DefaultIfEmpty(),
                            (x, y) => new
                            {
                                x.NomenclatureId,
                                Quantity = y == null ? 0.0m : y.Quantity
                            })
                        .ToArray();
                    var newRecords = new List<CalcSheet>();
                    foreach (var rec in newBalances)
                    {
                        DateTime now = DateTime.Now;
                        var newBalance = new CalcSheet
                        {
                            NomenclatureId = rec.NomenclatureId,
                            GasStationId = station.Id,
                            Quantity = rec.Quantity,
                            LastUpdate = now
                        };
                        newRecords.Add(newBalance);
                    }

                    await _context.CalcSheets.AddRangeAsync(newRecords);

                    foreach (var rec in newRecords)
                    {
                        var historyRecord = CalcSheetHistory.CreateHistoryRecord(rec, rec.LastUpdate);
                        await _context.CalcSheetHistories.AddAsync(historyRecord);
                    }

                    await _context.SaveChangesAsync();
                    processingLog.AppendLine($"Добавлено записей: {newBalances.Length}");

                    currentRow += totalNomenclatures;
                    currentStation++;
                    progressIndicator.Progress = 100.0m * currentRow / totalRows;
                    _coordinator.AddOrUpdate(progressIndicator);
                }

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
        /// Рассчитать потребность в заказе Номенклатуры
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
                processingLog.AppendLine("Расчет потребности в заказе ТМЦ начат.");

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

                    foreach (var id in portion)
                    {
                        await CalculateSingleOrder(id);
                    }

                    await _context.SaveChangesAsync();

                    currentRow += 100;
                    progressIndicator.Progress = 100.0m * currentRow / totalRows;
                    _coordinator.AddOrUpdate(progressIndicator);
                }

                processingLog.AppendLine($"Расчет потребности в заказе ТМЦ завершен");
                var successFinalProgress = new BackgroundServiceProgress
                {
                    Key = serviceKey,
                    Status = BackgroundServiceStatus.RanToCompletion,
                    Step = "Расчет потребности в заказе ТМЦ завершен",
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
        /// Рассчитать потребность в заказе отдельной позиции Номенклатуры по АЗС
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> CalculateSingleOrder(int id)
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
                    .Include(x => x.ServiceLevel)
                    .Include(x => x.OperatorRoomFormat)
                    .Include(x => x.TradingHallOperatingMode)
                    .Include(x => x.CashboxLocation)
                    .Include(x => x.TradingHallSize)
                    .FirstOrDefaultAsync(x => x.Id == nomBalance.GasStationId);

                ExpressionEvaluator evaluator = new ExpressionEvaluator();
                evaluator.Variables = new Dictionary<string, object>()
                {
                    { "if", new Func<bool,double,double,double>((c, x, y) => c ? x : y)},
                    { "cluster", gasStation.ServiceLevel.Name.ToLower() },
                    { "operformat", gasStation.OperatorRoomFormat.Name.ToLower() },
                    { "regime", gasStation.TradingHallOperatingMode.Name.ToLower() },
                    { "cashdesk", gasStation.CashboxLocation.Name.ToLower() },
                    { "salearea", gasStation.TradingHallSize.Name.ToLower() },
                    { "totalarm", gasStation.CashboxTotal },
                    { "totaltrk", gasStation.FuelDispenserTotal },
                    { "avgcheck", Convert.ToDouble(gasStation.ChequePerDay) },
                    { "totalclienttoiletroom", gasStation.ClientRestroomTotal },
                    { "tambour", gasStation.HasJointRestroomEntrance },
                    { "monthincome", Convert.ToDouble(gasStation.RevenueAvg) },
                    { "sibilla", gasStation.HasSibilla },
                    { "bakery", gasStation.HasBakery },
                    { "cakes", gasStation.HasCakes },
                    { "fries", gasStation.DeepFryTotal },
                    { "marmite", gasStation.HasMarmite },
                    { "kitchen", gasStation.HasKitchen },
                    { "coffeemachine", gasStation.CoffeeMachineTotal },
                    { "totalpersonal", gasStation.PersonnelPerDay },
                    { "avgbandlength", Convert.ToDouble(gasStation.ChequeBandLengthPerDay) },
                    { "imagecoef", Convert.ToDouble(gasStation.RepresentativenessFactor) }
                };

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
            }

            if (nomBalance.Plan == plan)
            {
                return true;
            }

            DateTime now = DateTime.Now;
            nomBalance.Plan = plan;
            nomBalance.LastUpdate = now;
            _context.Entry(nomBalance).Property(r => r.Plan).IsModified = true;
            _context.Entry(nomBalance).Property(r => r.LastUpdate).IsModified = true;
            var historyRecord = CalcSheetHistory.CreateHistoryRecord(nomBalance, now);
            _context.CalcSheetHistories.Add(historyRecord);

            return true;
        }
    }
}
