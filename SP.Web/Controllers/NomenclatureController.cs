using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class NomenclatureController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMasterService _masterService;
        private readonly IAppLogger _appLogger;

        public NomenclatureController(IInventoryService inventoryService, IMasterService masterService, IAppLogger appLogger)
        {
            _inventoryService = inventoryService;
            _masterService = masterService;
            _appLogger = appLogger;
        }

        [Route("[controller]/Index")]
        public async Task<IActionResult> IndexAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "nomenclature", "Открыт справочник Номенклатура.");

            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            var groupList = new SelectList(nomenclatureGroups, "Id", "Name").ToList();
            groupList.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["NomenclatureGroupList"] = groupList;

            return View("Index");
        }

        [HttpPost]
        [Route("[controller]/LoadList")]
        public async Task<IActionResult> LoadListAsync(string groups)
        {
            var groupIdList = groups.SplitToIntArray();
            var list = await _inventoryService.GetNomenclatureListAsync(groupIdList);

            return Json(new { data = list });
        }

        public async Task<IActionResult> LoadNomenclature(string groups, string useful)
        {
            var groupIdList = groups.SplitToIntArray();
            var usefulList = useful.SplitToIntArray();
            var list = await _inventoryService.GetNomenclatureListItemsAsync(groupIdList, usefulList);

            return Json(list);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _inventoryService.GetNomenclatureModelAsync(id);

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "nomenclature", 
                $"Cправочник Номенклатура. Открыта запись код {model.Code}, {model.Name}");

            var measureUnits = await _masterService.GetDictionaryListAsync<MeasureUnit>();
            ViewData["MeasureUnitList"] = new SelectList(measureUnits, "Id", "Name").ToList(); ;
            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            ViewData["NomenclatureGroupList"] = new SelectList(nomenclatureGroups, "Id", "Name").ToList(); ;

            return View("_Edit", model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new NomenclatureModel();

            var measureUnits = await _masterService.GetDictionaryListAsync<MeasureUnit>();
            ViewData["MeasureUnitList"] = new SelectList(measureUnits, "Id", "Name").ToList(); ;
            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            ViewData["NomenclatureGroupList"] = new SelectList(nomenclatureGroups, "Id", "Name").ToList(); ;

            return View("_Edit", model);
        }

        public async Task<IActionResult> SaveAsync([FromForm] NomenclatureModel model)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(ms => ms.Errors)
                    .Select(e => e.ErrorMessage);
                errorMessage = string.Join(" ", errors);
                return Content(errorMessage);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена";
            var result = await _inventoryService.SaveNomenclatureAsync(model);
            if (result.Success)
            {
                await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "nomenclature",
                    $"Cправочник Номенклатура. {actionVerb} запись код {model.Code}, {model.Name}");

                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> LoadInventoryList(int id)
        {
            var list = await _inventoryService.GetNomenclatureInventoryListAsync(id);

            return Json(new { data = list });
        }
    }
}