using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    public class MasterController : Controller
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        public async Task<IActionResult> Region()
        {
            var regions = await _masterService.GetRegionListAsync();
            var list = new SelectList(regions, "Id", "Name").ToList();
            list.Insert(0, new SelectListItem("-- ВСЕ --", ""));

            ViewData["Regions"] = list;

            return View("Region");
        }

        [HttpPost]
        public async Task<IActionResult> LoadTerritoryList()
        {
            var territories = await _masterService.GetTerritoryListAsync();

            return Json(new { data = territories });
        }

        public IActionResult CreateRegion()
        {
            var model = new RegionModel();

            return View("_RegionEdit", model);
        }

        public async Task<IActionResult> EditRegionAsync(int id)
        {
            var model = await _masterService.GetRegionAsync(id);

            return View("_RegionEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRegionAsync([FromForm] RegionModel model)
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

            var result = await _masterService.SaveRegionAsync(model);
            if (result.Success)
            {
                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }
    }
}