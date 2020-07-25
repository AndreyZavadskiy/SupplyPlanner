using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
    }

    public class BackgroundInventoryService : IBackgroundInventoryService
    {
        private readonly IBackgroundCoordinator _coordinator;
        private readonly IExcelParser _parser;
        private readonly IMasterService _masterService;
        private readonly IGasStationService _stationService;
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public BackgroundInventoryService(IBackgroundCoordinator coordinator, IExcelParser parser, 
            IMasterService masterService, IGasStationService stationService, IInventoryService inventoryService)
        {
            _coordinator = coordinator;
            _parser = parser;
            _masterService = masterService;
            _stationService = stationService;
            _inventoryService = inventoryService;
        }

        public BackgroundInventoryService(IBackgroundCoordinator coordinator)
        {
            _coordinator = coordinator;
            _parser = new ExcelParser();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var options = optionsBuilder
                .UseSqlServer("Server=(local);Database=SupplyPlanner;Trusted_Connection=True;MultipleActiveResultSets=true")
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

            var person = await _masterService.GetPersonAsync(aspNetUserId);

            int totalFiles = files.Count;
            int fileCount = 1;
            StringBuilder processingLog = new StringBuilder();

            foreach (var file in files)
            {
                string stepMessageTemplate = "Парсинг файла [" + fileCount + "/" + totalFiles + "]: \"{0}\", лист: \"{1}\"";
                try
                {
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

                            int lastRow = ws.Dimension.End.Row;
                            parsedData = new List<ParsedInventory>(lastRow - headerRow);
                            // извлекаем данные из листа Excel и сохраняем в списке
                            for (int r = headerRow + 1; r <= lastRow; r++)
                            {
                                var parsedRowData = _parser.ParseDataRow(ws, wsColumns, r);
                                if (parsedRowData?.Count > 0)
                                {
                                    if (decimal.TryParse(parsedRowData["Quantity"], NumberStyles.Number, CultureInfo.CurrentCulture, out decimal qty))
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

                            var stations = await _stationService.GetGasStationIdentificationListAsync();
                            var measureUnits = await _masterService.GetDictionaryListAsync<MeasureUnit>();
                            // стыковка справочника АЗС
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

                            var emptyStations = dataWithStation
                                .Where(x => x.GasStationId == null)
                                .Select(x => new {x.StationCodeSAP, x.StationPetronicsName})
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
                                processingLog.AppendLine("Удаляем предыдущие остатки ТМЦ.");
                                await _inventoryService.PurgeStageInventoryAsync(person.Id);
                                processingLog.AppendLine("Выполняем сохранение остатков ТМЦ.");
                                await _inventoryService.SaveStageInventoryAsync(inventoryData, serviceKey, person.Id);
                                processingLog.AppendLine($"Сохранено {inventoryData.Length} записей.");
                            }
                            else
                            {
                                processingLog.AppendLine("Отсутствуют данные для загрузки остатков ТМЦ.");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
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

            return true;
        }

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

        public async Task<bool> AutoMergeAsync(Guid serviceKey, string aspNetUserId)
        {
            throw new NotImplementedException();
        }
    }
}
