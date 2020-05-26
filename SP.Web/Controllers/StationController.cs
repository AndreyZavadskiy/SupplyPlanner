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
    public class StationController : Controller
    {
        private readonly IGasStationService _gasStationService;
        private readonly IMasterService _masterService;

        public StationController(IGasStationService gasStationService, IMasterService masterService)
        {
            _gasStationService = gasStationService;
            _masterService = masterService;
        }

        public async Task<IActionResult> Index()
        {
            var regions = await _masterService.SelectRegionAsync();
            ViewData["Regions"] = new SelectList(regions, "Id", "Name").ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadList(int? region, int? terr)
        {
            IEnumerable<GasStationModel> stations;
            if (region == null)
            {
                stations = await _gasStationService.GetGasStationList(null);
            }
            else if (terr == null)
            {
                stations = new GasStationModel[0];
            }
            else
            {
                stations = await _gasStationService.GetGasStationList(terr);
            }

            return Json(new { data = stations });
        }
    }
}