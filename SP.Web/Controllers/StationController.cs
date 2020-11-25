using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
using SP.Data;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class StationController : Controller
    {
        private readonly IGasStationService _gasStationService;
        private readonly IMasterService _masterService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLogger _appLogger;

        public StationController(IGasStationService gasStationService, IMasterService masterService, UserManager<ApplicationUser> userManager, IAppLogger appLogger)
        {
            _gasStationService = gasStationService;
            _masterService = masterService;
            _userManager = userManager;
            _appLogger = appLogger;
        }

        public async Task<IActionResult> Index(string regions, string terrs)
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation", "Открыт справочник АЗС.");
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);

            var regionList = currentRoles.Contains("RegionalManager")
                ? await _masterService.SelectRegionAsync(person.Id)
                : await _masterService.SelectRegionAsync();

            var list = new SelectList(regionList, "Id", "Name").ToList();
            list.Add(new SelectListItem("ВСЕ", Int32.MaxValue.ToString()));
            ViewData["RegionList"] = list;
            ViewData["SelectedRegions"] = regions;
            ViewData["SelectedTerritories"] = terrs;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadList(string regions, string terrs)
        {
            if (!string.IsNullOrWhiteSpace(regions) && regions.Contains("2147483647"))
            {
                regions = null;
            }

            int[] regionIdList = regions.SplitToIntArray();
            int[] terrIdList = terrs.SplitToIntArray();

            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);
            int? personId = null;
            if (currentRoles.Contains("RegionalManager"))
            {
                personId = person.Id;
            }

            var stations = await GetGasStationListItems(regionIdList, terrIdList, personId);

            return Json(new { data = stations });
        }

        public async Task<IActionResult> LoadStations(string regions, string terrs)
        {
            int[] regionIdList = regions.SplitToIntArray();
            int[] terrIdList = terrs.SplitToIntArray();
            var list = await GetGasStationListItems(regionIdList, terrIdList);
            var stations = list
                .OrderBy(x => x.StationNumber)
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.StationNumber
                });

            return Json(stations);
        }

        private async Task<IEnumerable<GasStationListItem>> GetGasStationListItems(int[] regions, int[] territories, int? personId = null)
        {
            IEnumerable<GasStationListItem> stations;
            if (regions == null && territories == null)
            {
                stations = new GasStationListItem[0];
            }
            else if (personId != null)
            {
                stations = await _gasStationService.GetGasStationListAsync(regions, territories, personId.Value);
            }
            else 
            {
                stations = await _gasStationService.GetGasStationListAsync(regions, territories);
            }

            return stations;
        }

        [Route("Station/{id}")]

        public async Task<IActionResult> GetAsync(int id, string regions, string terrs)
        {
            var station = await _gasStationService.GetGasStationAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation",
                $"Открыта запись АЗС {station.StationNumber}");

            return await PrepareGasStationView(station, regions, terrs);
        }

        private async Task<IActionResult> PrepareGasStationView(GasStationModel model, string regions, string terrs)
        {
            var regionList = await _masterService.SelectRegionAsync();
            ViewData["RegionList"] = new SelectList(regionList, "Id", "Name");
            var territoryList = await _masterService.SelectTerritoryAsync(new int[] { model.RegionId });
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

            ViewData["SelectedRegions"] = regions;
            ViewData["SelectedTerritories"] = terrs;

            return View("GasStation", model);
        }

        [Route("Station/Create")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateAsync(string regions, string terrs)
        {
            var model = new GasStationModel();
            return await PrepareGasStationView(model, regions, terrs);
        }

        /// <summary>
        /// Сохранить АЗС и вернуться в список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,RegionalManager")]
        public async Task<IActionResult> EditAsync([FromForm] GasStationModel model, [FromForm] string regions, [FromForm] string terrs)
        {
            return await SaveGasStation(model, regions, terrs, actionName: "Index");
        }

        /// <summary>
        /// Сохранить АЗС
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,RegionalManager")]
        public async Task<IActionResult> SaveAsync([FromForm] GasStationModel model, [FromForm] string regions, [FromForm] string terrs)
        {
            return await SaveGasStation(model, regions, terrs);
        }

        /// <summary>
        /// Сохранить запись АЗС
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveGasStation(GasStationModel model, string regions, string terrs, string actionName = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareGasStationView(model, regions, terrs);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена"; 
            var result = await _gasStationService.SaveGasStationAsync(model);

            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareGasStationView(model, regions, terrs);
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation",
                $"{actionVerb} запись АЗС {model.StationNumber}");

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return Redirect($"/Station?regions={regions}&terrs={terrs}");
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/Station/{result.Id}?regions={regions}&terrs={terrs}");
        }

    }
}