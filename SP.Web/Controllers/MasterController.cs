using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.ViewModels;

namespace SP.Web.Controllers
{
    public class MasterController : Controller
    {
        private readonly IMasterService _masterService;

        public MasterController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        /// <summary>
        /// Вывести форму редактирования регионов и территорий
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Region()
        {
            var regions = await _masterService.GetRegionListAsync();
            var list = new SelectList(regions, "Id", "Name").ToList();
            list.Insert(0, new SelectListItem("-- ВСЕ --", ""));

            ViewData["Regions"] = list;

            return View("Region");
        }

        public async Task<IActionResult> LoadRegionList()
        {
            var regions = await _masterService.GetRegionListAsync();

            return Json(regions);
        }

        /// <summary>
        /// Получить список территорий
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadTerritoryList()
        {
            var territories = await _masterService.GetTerritoryListAsync();

            return Json(new { data = territories });
        }

        /// <summary>
        /// Вывести форму создания региона
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateRegion()
        {
            var model = new RegionModel();

            return View("_RegionEdit", model);
        }

        /// <summary>
        /// Вывести форму редактирования региона
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditRegionAsync(int id)
        {
            var model = await _masterService.GetRegionAsync(id);
            if (model == null)
            {
                // TODO: вывести сообщение об ошибке
                return null;
            }

            return View("_RegionEdit", model);
        }

        /// <summary>
        /// Сохранить регион
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        public async Task<IActionResult> CreateTerritoryAsync(int parent)
        {
            var parentRegion = await _masterService.GetRegionAsync(parent);
            if (parentRegion == null)
            {
                // TODO: вывести сообщение об ошибке
                return null;
            }
            
            var model = new TerritoryViewModel
            {
                Region = parentRegion,
                Territory = new RegionModel
                {
                    ParentId = parent
                }
            };

            return View("_TerritoryEdit", model);
        }

        public async Task<IActionResult> EditTerritoryAsync(int id)
        {
            var territory = await _masterService.GetRegionAsync(id);
            if (territory == null || territory.ParentId == null)
            {
                // TODO: вывести сообщение об ошибке
                return null;
            }
            var region = await _masterService.GetRegionAsync(territory.ParentId.Value);
            var model = new TerritoryViewModel
            {
                Region = region,
                Territory = territory
            };

            return View("_TerritoryEdit", model);

        }

        [HttpPost]
        public async Task<IActionResult> EditTerritoryAsync([FromForm] TerritoryViewModel model)
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

            var result = await _masterService.SaveRegionAsync(model.Territory);
            if (result.Success)
            {
                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }
    }
}