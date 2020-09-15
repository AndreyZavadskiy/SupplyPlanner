﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
using SP.Data;
using SP.Service.Background;
using SP.Service.DTO;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Utility;
using SP.Web.ViewModels;

namespace SP.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMasterService _masterService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLogger _appLogger;

        public InventoryController(IInventoryService inventoryService, IMasterService masterService, UserManager<ApplicationUser> userManager, IAppLogger appLogger)
        {
            _inventoryService = inventoryService;
            _masterService = masterService;
            _userManager = userManager;
            _appLogger = appLogger;
        }

        /// <summary>
        /// Отобразить форму для загрузки остатков ТМЦ
        /// </summary>
        /// <returns></returns>
        public IActionResult Upload()
        {
            var model = new UploadInventoryViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("Upload", model);
        }

        /// <summary>
        /// Запустить загрузку остатков ТМЦ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="coordinator"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/Upload")]
        public async Task<IActionResult> UploadAsync([FromForm]UploadInventoryViewModel model,
            [FromServices] IBackgroundCoordinator coordinator)
        {
            if (model.Files == null)
            {
                return null;
            }

            string fileNameList = string.Join(", ", model.Files.Select(x => x.FileName));
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory", $"Запущена загрузка остатков ТМЦ. Файлы: {fileNameList}.");

            // записываем во временные файлы на сервере
            var uploadedFiles = new List<UploadedFile>();
            foreach (var formFile in model.Files)
            {
                if (formFile.Length == 0 || !CheckExcelFile(formFile.FileName))
                {
                    continue;
                }

                string filePath = Path.GetTempFileName();
                await using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                uploadedFiles.Add(new UploadedFile
                {
                    FileName = formFile.FileName,
                    FileInfo = new FileInfo(filePath)
                });
            }

            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundUpload(coordinator, serviceKey, uploadedFiles, user.Id);

            return Json(new { Key = serviceKey });
        }

        /// <summary>
        /// Отобразить форму для автоматического объединения ТМЦ с Номенклатурой
        /// </summary>
        /// <returns></returns>
        public IActionResult AutoMerge()
        {
            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("AutoMerge", model);
        }

        [HttpPost]
        [Route("[controller]/AutoMerge")]
        public async Task<IActionResult> AutoMergeAsync([FromForm] InventoryProcessingViewModel model,
            [FromServices] IBackgroundCoordinator coordinator)
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory", $"Запущено автоматическое объединение остатков ТМЦ.");

            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundAutoMerge(coordinator, serviceKey, user.Id);

            return Json(new { Key = serviceKey });
        }

        /// <summary>
        /// Отобразить форму для ручного объединения ТМЦ с Номенклатурой
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManualMergeAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory", 
                "Открыт список ТМЦ для ручного объединения с Номенклатурой.");

            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("ManualMerge", model);
        }

        /// <summary>
        /// Получить список ТМЦ для ручного объединения с Номенклатурой
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadMergeListAsync()
        {
            var inventoryList = await _inventoryService.GetListForManualMerge();

            return Json(new { data = inventoryList });
        }

        /// <summary>
        /// Отобразить форму для выбора Номенклатуры
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult NomenclatureForm(int id)
        {
            return View("_SelectNomenclature");
        }

        [HttpPost]
        [Route("[controller]/LinkNomenclature")]
        public async Task<IActionResult> LinkNomenclatureAsync([FromBody] InventoryLinkViewModel model)
        {
            if (model == null || model.Inventories == null || model.Inventories.Length == 0)
            {
                return Json(new { updated = 0 });
            }

            int updated = await _inventoryService.LinkInventoryToNomenclatureAsync(model.Inventories, model.Nomenclature);

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory", 
                $"Объединено {updated} позиций ТМЦ с номенклатурой id {model.Nomenclature}.");

            return Json(new { updated });
        }

        [HttpPost]
        [Route("[controller]/BlockInventory")]
        public async Task<IActionResult> BlockInventoryAsync([FromBody] InventoryLinkViewModel model)
        {
            if (model == null || model.Inventories.Length == 0)
            {
                return Json(new { updated = 0 });
            }

            int updated = await _inventoryService.BlockInventoryAsync(model.Inventories);

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                $"Исключено {updated} позиций ТМЦ из объединения в Номенклатуру.");

            return Json(new { updated });
        }

        /// <summary>
        /// Отобразить форму для расчета остатков ТМЦ
        /// </summary>
        /// <returns></returns>
        public IActionResult CalcBalance()
        {
            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("CalcBalance", model);
        }

        [HttpPost]
        [Route("[controller]/CalcBalance")]
        public async Task<IActionResult> CalcBalanceAsync([FromServices] IBackgroundCoordinator coordinator)
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                "Запущен расчет остатков по Номенклатуре.");

            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundBalanceCalculation(coordinator, serviceKey, user.Id);

            return Json(new { Key = serviceKey });
        }

        /// <summary>
        /// Отобразить форму остатков по Номенклатуре
        /// </summary>
        /// <returns></returns>
        [Route("[controller]/Balance")]
        public async Task<IActionResult> BalanceAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                "Открыт список остатков по Номенклатуре.");

            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            await LoadEssentialDictionaries();

            return View("Balance", model);
        }

        private async Task LoadEssentialDictionaries()
        {
            var regions = await _masterService.SelectRegionAsync();
            var list = new SelectList(regions, "Id", "Name").ToList();
            list.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["RegionList"] = list;

            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            var groupList = new SelectList(nomenclatureGroups, "Id", "Name").ToList();
            groupList.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["NomenclatureGroupList"] = groupList;
        }

        /// <summary>
        /// Получить остатки ТМЦ
        /// </summary>
        /// <param name="region"></param>
        /// <param name="terr"></param>
        /// <param name="station"></param>
        /// <param name="group"></param>
        /// <param name="nom"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom)
        {
            if (region == null && terr == null && group == null)
            {
                var zeroItem = new BalanceListItem { Name = "Установите фильтры для отображения данных" };
                return Json(new { data = new[] { zeroItem } });
            }

            var list = await _inventoryService.GetBalanceListAsync(region, terr, station, group, nom);

            return Json(new { data = list });
        }

        [Route("[controller]/Demand")]
        public async Task<IActionResult> DemandAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                "Открыт список для заказа ТМЦ по Номенклатуре.");

            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            await LoadEssentialDictionaries();

            return View("Demand", model);
        }

        [HttpPost]
        public async Task<IActionResult> LoadDemandList(int? region, int? terr, int? station, int? group, int? nom)
        {
            if (region == null && terr == null && group == null)
            {
                var zeroItem = new DemandListItem { Name = "Установите фильтры для отображения данных" };
                return Json(new { data = new[] { zeroItem } });
            }

            var list = await _inventoryService.GetDemandListAsync(region, terr, station, group, nom);
            return Json(new { data = list });
        }

        /// <summary>
        /// Рассчитать заказ
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="coordinator"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/CalcOrder")]
        public async Task<IActionResult> CalcOrderAsync([FromBody] int[] idList,
            [FromServices] IBackgroundCoordinator coordinator)
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                "Запущен автоматический расчет заказа ТМЦ по Номенклатуре.");

            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundOrderCalculation(coordinator, serviceKey, idList, user.Id);

            return Json(new { Key = serviceKey });
        }

        [HttpPost]
        [Route("[controller]/Requirement")]
        public async Task<IActionResult> Requirement([FromBody] RequirementViewModel model)
        {
            decimal? fixedAmount = null;
            if (decimal.TryParse(model.FixedAmount, out decimal amount))
            {
                fixedAmount = amount;
            }

            if ((fixedAmount == null && string.IsNullOrWhiteSpace(model.Formula)) || model.IdList.Length == 0)
            {
                var badResult = new RequirementViewModel
                {
                    UpdatedCount = 0
                };
                return Json(badResult);
            }

            if (fixedAmount != null && !string.IsNullOrWhiteSpace(model.Formula))
            {
                model.Formula = null;
            }

            int updated = await _inventoryService.SetRequirementAsync(fixedAmount, model.Formula, model.IdList);
            var successResult = new RequirementViewModel
            {
                FixedAmount = fixedAmount?.ToString(),
                Formula = model.Formula,
                UpdatedCount = updated
            };

            return Json(successResult);
        }

        [HttpPost]
        [Route("[controller]/SaveOrder")]
        public async Task<IActionResult> SaveOrderAsync([FromBody] IEnumerable<OrderQuantity> data)
        {
            if (data == null || !data.Any())
            {
                return Json(new { order = 0, updated = 0 });
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory", 
                $"Сохранен заказ ТМЦ по Номенклатуре, количество позиций {data.Count()}.");

            var user = await _userManager.GetUserAsync(User);
            var person = await _masterService.GetPersonAsync(user.Id);
            var result = await _inventoryService.SaveOrderAsync(data, person.Id);

            return Json(new { order = result.OrderNumber, updated = result.RecordCount });
        }

        [Route("[controller]/Order")]
        public async Task<IActionResult> OrderAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "inventory",
                "Открыт список заказов ТМЦ по Номенклатуре.");

            var model = new InventoryProcessingViewModel
            {
                ProcessingDate = DateTime.Now
            };

            await LoadEssentialDictionaries();

            return View("Order", model);
        }
        public async Task<IActionResult> LoadOrderList()
        {
            var data = await _inventoryService.GetOrderListAsync();
            var list = data
                .Select(x => new
                {
                    x.Id,
                    OrderDate = x.OrderDate.AddMilliseconds(x.OrderDate.Millisecond),
                    x.PersonName
                });
            return Json(new { data = list });
        }
        public async Task<IActionResult> LoadOrderDetailList(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var list = await _inventoryService.GetOrderDetailAsync(id.Value);
            return Json(new { data = list });
        }

        /// <summary>
        /// Получить статус выполнения задания
        /// </summary>
        /// <param name="key"></param>
        /// <param name="coordinator"></param>
        /// <returns></returns>
        public IActionResult PeekStatus(Guid key, [FromServices] IBackgroundCoordinator coordinator)
        {
            var data = coordinator.GetProgress(key);

            if (data.Status == BackgroundServiceStatus.NotFound)
            {
                return Json(new
                {
                    status = 0,
                    step = "Загрузка ТМЦ уже выполнена либо указан неправильный идентификатор загрузки.",
                    progress = 0
                });
            }

            return Json(new
            {
                status = data.Status,
                step = data.Step,
                progress = data.Progress,
                log = data.Log
            });
        }

        private bool CheckExcelFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string fileExtension = Path.GetExtension(fileName);
            switch (fileExtension.ToLower())
            {
                case ".xlsx":
                    return true;
            }

            return false;
        }

        private void StartBackgroundUpload(IBackgroundCoordinator coordinator, Guid serviceKey, List<UploadedFile> files, string aspNetUserId)
        {
            Task.Run(async () =>
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.UploadAsync(serviceKey, files, aspNetUserId);
            });
        }

        private void StartBackgroundAutoMerge(IBackgroundCoordinator coordinator, Guid serviceKey, string aspNetUserId)
        {
            Task.Run(async () =>
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.AutoMergeAsync(serviceKey, aspNetUserId);
            });
        }

        private void StartBackgroundBalanceCalculation(IBackgroundCoordinator coordinator, Guid serviceKey, string aspNetUserId)
        {
            Task.Run(async () =>
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.CalculateBalanceAsync(serviceKey, aspNetUserId);
            });
        }

        private void StartBackgroundOrderCalculation(IBackgroundCoordinator coordinator, Guid serviceKey, int[] idList, string aspNetUserId)
        {
            Task.Run(async () =>
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.CalculateOrderAsync(serviceKey, idList, aspNetUserId);
            });
        }
    }
}