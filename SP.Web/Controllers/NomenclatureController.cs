using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    public class NomenclatureController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMasterService _masterService;

        public NomenclatureController(IInventoryService inventoryService, IMasterService masterService)
        {
            _inventoryService = inventoryService;
            _masterService = masterService;
        }

        [Route("[controller]/Index")]
        public async Task<IActionResult> IndexAsync()
        {
            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            var groupList = new SelectList(nomenclatureGroups, "Id", "Name").ToList();
            groupList.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["NomenclatureGroupList"] = groupList;

            return View("Index");
        }

        [HttpPost]
        [Route("[controller]/LoadList")]
        public async Task<IActionResult> LoadListAsync()
        {
            var list = await _inventoryService.GetNomenclatureListAsync();

            return Json(new { data = list });
        }

        public async Task<IActionResult> LoadNomenclature(int group)
        {
            var list = await _inventoryService.GetNomenclatureListItemsAsync(group);

            return Json(list);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _inventoryService.GetNomenclatureModelAsync(id);

            var measureUnits = await _masterService.GetDictionaryListAsync<MeasureUnit>();
            ViewData["MeasureUnitList"] = new SelectList(measureUnits, "Id", "Name").ToList(); ;
            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            ViewData["NomenclatureGroupList"] = new SelectList(nomenclatureGroups, "Id", "Name").ToList(); ;

            return View("_Edit", model);
        }
    }
}