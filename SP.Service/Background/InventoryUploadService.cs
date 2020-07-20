﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;
using SP.Core.Master;
using SP.Data;
using SP.Service.DTO;
using SP.Service.Excel;
using SP.Service.Services;

namespace SP.Service.Background
{
    public interface IInventoryUploadService
    {
        bool Run(Guid serviceKey, List<UploadedFile> files);
    }

    public class InventoryUploadService : IInventoryUploadService
    {
        private readonly IBackgroundCoordinator _coordinator;
        private readonly IExcelParser _parser;
        private readonly IMasterService _masterService;
        private readonly IGasStationService _stationService;
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public InventoryUploadService(IBackgroundCoordinator coordinator, IExcelParser parser, 
            IMasterService masterService, IGasStationService stationService, IInventoryService inventoryService)
        {
            _coordinator = coordinator;
            _parser = parser;
            _masterService = masterService;
            _stationService = stationService;
            _inventoryService = inventoryService;
        }

        public InventoryUploadService(IBackgroundCoordinator coordinator)
        {
            _coordinator = coordinator;
            _parser = new ExcelParser();
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var options = optionsBuilder
                .UseSqlServer("Server=(local);Database=SupplyPlanner;Trusted_Connection=True;MultipleActiveResultSets=true")
                .UseLoggerFactory(ApplicationDbContext.ApplicationDbLoggerFactory)
                .EnableSensitiveDataLogging()
                .Options;
            _context = new ApplicationDbContext(options);
            _masterService = new MasterService(_context);
            _stationService = new GasStationService(_context);
            _inventoryService = new InventoryService(_context);
        }

        public bool Run(Guid serviceKey, List<UploadedFile> files)
        {
            // регистрируем в координаторе
            var data = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.Created,
                Progress = 0
            };
            
            _coordinator.AddOrUpdate(data);

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

                            // стыковка справочника АЗС и единиц измерения
                            var stations = _stationService.GetGasStationIdentificationListAsync().Result.ToArray();
                            var measureUnits = _masterService.GetDictionaryListAsync<MeasureUnit>().Result.ToArray();

                            var dataWithStationId = parsedData
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

                            var inventoryData = dataWithStationId
                                .GroupJoin(measureUnits,
                                    d => d.StationCodeSAP,
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

                            // пакетное сохранение в БД
                            _inventoryService.PurgeStageInventoryAsync();
                            _inventoryService.SaveStageInventoryAsync(inventoryData);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                fileCount++;
            }

            var finalProgress = new BackgroundServiceProgress
            {
                Key = serviceKey,
                Status = BackgroundServiceStatus.RanToCompletion,
                Step = "Загрузка завершена",
                Progress = 100,
                Log = "Finished"
            };
            _coordinator.AddOrUpdate(finalProgress);

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
    }
}