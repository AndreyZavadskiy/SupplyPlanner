using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
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

        public async Task<IActionResult> Index(int? region, int? terr)
        {
            var regions = await _masterService.SelectRegionAsync();
            ViewData["RegionList"] = new SelectList(regions, "Id", "Name").ToList();
            ViewData["SelectedRegion"] = region;
            ViewData["SelectedTerritory"] = terr;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadList(int? region, int? terr)
        {
            var stations = await GetGasStationListItems(region, terr);

            return Json(new { data = stations });
        }

        public async Task<IActionResult> LoadStations(int? region, int? terr)
        {
            var list = await GetGasStationListItems(region, terr);
            var stations = list
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.StationNumber
                });

            return Json(stations);
        }

        private async Task<IEnumerable<GasStationListItem>> GetGasStationListItems(int? region, int? terr)
        {
            IEnumerable<GasStationListItem> stations;
            if (region == null)
            {
                stations = await _gasStationService.GetGasStationListAsync(null);
            }
            else if (terr == null)
            {
                stations = new GasStationListItem[0];
            }
            else
            {
                stations = await _gasStationService.GetGasStationListAsync(terr);
            }

            return stations;
        }

        [Route("Station/{id}")]

        public async Task<IActionResult> GetAsync(int id)
        {
            var station = await _gasStationService.GetGasStationAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            return await PrepareGasStationView(station);
        }

        private async Task<IActionResult> PrepareGasStationView(GasStationModel model)
        {
            var regionList = await _masterService.SelectRegionAsync();
            ViewData["RegionList"] = new SelectList(regionList, "Id", "Name");
            var territoryList = await _masterService.SelectTerritoryAsync(model.RegionId);
            ViewData["TerritoryList"] = new SelectList(territoryList, "Id", "Name");
            var settlementList = await _masterService.GetDictionaryListAsync<Settlement>();
            ViewData["SettlementList"] = new SelectList(settlementList, "Id", "Name");
            var stationLocationList = await _masterService.GetDictionaryListAsync<StationLocation>();
            ViewData["StationLocationList"] = new SelectList(stationLocationList, "Id", "Name");
            var stationStatusList = await _masterService.GetDictionaryListAsync<StationStatus>();
            ViewData["StationStatusList"] = new SelectList(stationStatusList, "Id", "Name");
            var serviceLevelList = await _masterService.GetDictionaryListAsync<ServiceLevel>();
            ViewData["ServiceLevelList"] = new SelectList(serviceLevelList, "Id", "Name");
            var operatorRoomFormatList = await _masterService.GetDictionaryListAsync<OperatorRoomFormat>();
            ViewData["OperatorRoomFormatList"] = new SelectList(operatorRoomFormatList, "Id", "Name");
            var managementSystemList = await _masterService.GetDictionaryListAsync<ManagementSystem>();
            ViewData["ManagementSystemList"] = new SelectList(managementSystemList, "Id", "Name");
            var tradingHallOperatingModeList = await _masterService.GetDictionaryListAsync<TradingHallOperatingMode>();
            ViewData["TradingHallOperatingModeList"] = new SelectList(tradingHallOperatingModeList, "Id", "Name");
            var clientRestroomList = await _masterService.GetDictionaryListAsync<ClientRestroom>();
            ViewData["ClientRestroomList"] = new SelectList(clientRestroomList, "Id", "Name");
            var cashboxLocationList = await _masterService.GetDictionaryListAsync<CashboxLocation>();
            ViewData["CashboxLocationList"] = new SelectList(cashboxLocationList, "Id", "Name");
            var tradingHallSizeList = await _masterService.GetDictionaryListAsync<TradingHallSize>();
            ViewData["TradingHallSizeList"] = new SelectList(tradingHallSizeList, "Id", "Name");

            ViewData["SelectedRegion"] = model.RegionId;
            ViewData["SelectedTerritory"] = model.TerritoryId;

            return View("GasStation", model);
        }

        [Route("Station/Create")]
        public async Task<IActionResult> CreateAsync(int region, int terr)
        {
            var model = new GasStationModel
            {
                RegionId = region,
                TerritoryId = terr
            };

            return await PrepareGasStationView(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([FromForm] GasStationModel model)
        {
            return await SaveGasStation(model, actionName: "Index");
        }

        /// <summary>
        /// Сохранить карточку пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveAsync([FromForm] GasStationModel model)
        {
            return await SaveGasStation(model);
        }

        private async Task<IActionResult> SaveGasStation(GasStationModel model, string actionName = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareGasStationView(model);
            }

            var result = await _gasStationService.SaveGasStationAsync(model);

            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareGasStationView(model);
            }

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return Redirect($"/Station?region={model.RegionId}&terr={model.TerritoryId}");
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/Station/{result.Id}");
        }
    }
}