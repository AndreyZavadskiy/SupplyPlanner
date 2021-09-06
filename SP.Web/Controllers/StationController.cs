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

        #region Gas stations

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
            if (!string.IsNullOrWhiteSpace(regions))
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
            var territoryList = await _masterService.SelectTerritoryAsync(regions.SplitToIntArray());
            ViewData["TerritoryList"] = new SelectList(territoryList, "Id", "Name");
            var settlementList = await _masterService.GetDictionaryListAsync<Settlement>();
            ViewData["SettlementList"] = new SelectList(settlementList, "Id", "Name");
            var stationLocationList = await _masterService.GetDictionaryListAsync<StationLocation>();
            ViewData["StationLocationList"] = new SelectList(stationLocationList, "Id", "Name");
            var stationStatusList = await _masterService.GetDictionaryListAsync<StationStatus>();
            ViewData["StationStatusList"] = new SelectList(stationStatusList, "Id", "Name");
            var segmentList = await _masterService.GetDictionaryListAsync<Segment>();
            ViewData["SegmentList"] = new SelectList(segmentList, "Id", "Name");
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
            var cashRegisterTapeList = await _masterService.GetDictionaryListAsync<CashRegisterTape>();
            ViewData["CashRegisterTapeList"] = new SelectList(cashRegisterTapeList, "Id", "Name");

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

        #endregion

        #region Fuel bases

        [Route("Station/FuelBase")]
        public async Task<IActionResult> FuelBase()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase", "Открыт справочник нефтебаз.");
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);

            return View("FuelBaseList");
        }

        [HttpPost]
        public async Task<IActionResult> LoadFuelBaseList()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);
            int? personId = null;

            var stations = await GetFuelBaseListItems(personId);

            return Json(new { data = stations });
        }

        private async Task<IEnumerable<FuelBaseListItem>> GetFuelBaseListItems(int? personId = null)
        {
            IEnumerable<FuelBaseListItem> stations;
            if (personId != null)
            {
                stations = await _gasStationService.GetFuelBaseListAsync(personId.Value);
            }
            else
            {
                stations = await _gasStationService.GetFuelBaseListAsync();
            }

            return stations;
        }

        [Route("Station/FuelBase/{id}")]

        public async Task<IActionResult> GetFuelBaseAsync(int id)
        {
            var station = await _gasStationService.GetFuelBaseAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase",
                $"Открыта запись {station.ObjectName}");

            return View("FuelBase", station);
        }

        [Route("Station/CreateFuelBase")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateFuelBaseAsync(string regions, string terrs)
        {
            var model = new FuelBaseModel();
            return View("FuelBase", model);
        }

        /// <summary>
        /// Сохранить нефтебазу и вернуться в список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditFuelBaseAsync([FromForm] FuelBaseModel model)
        {
            return await SaveFuelBase(model, actionName: "FuelBaseList");
        }

        /// <summary>
        /// Сохранить нефтебазу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveFuelBaseAsync([FromForm] FuelBaseModel model)
        {
            return await SaveFuelBase(model);
        }

        /// <summary>
        /// Сохранить запись нефтебазы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveFuelBase(FuelBaseModel model, string actionName = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("FuelBase", model);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена";
            var result = await _gasStationService.SaveFuelBaseAsync(model);

            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("FuelBase", model);
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation",
                $"{actionVerb} запись {model.ObjectName}");

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return Redirect($"/Station/FuelBase");
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/Station/FuelBase/{result.Id}");
        }

        #endregion

        #region Offices

        [Route("Station/Office")]
        public async Task<IActionResult> Office()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase", "Открыт справочник нефтебаз.");
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);

            return View("OfficeList");
        }

        [HttpPost]
        public async Task<IActionResult> LoadOfficeList()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);
            int? personId = null;

            var stations = await GetOfficeListItems(personId);

            return Json(new { data = stations });
        }

        private async Task<IEnumerable<OfficeListItem>> GetOfficeListItems(int? personId = null)
        {
            IEnumerable<OfficeListItem> stations;
            if (personId != null)
            {
                stations = await _gasStationService.GetOfficeListAsync(personId.Value);
            }
            else
            {
                stations = await _gasStationService.GetOfficeListAsync();
            }

            return stations;
        }

        [Route("Station/Office/{id}")]

        public async Task<IActionResult> GetOfficeAsync(int id)
        {
            var station = await _gasStationService.GetOfficeAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase",
                $"Открыта запись {station.ObjectName}");

            return View("Office", station);
        }

        [Route("Station/CreateOffice")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateOfficeAsync(string regions, string terrs)
        {
            var model = new OfficeModel();
            return View("Office", model);
        }

        /// <summary>
        /// Сохранить нефтебазу и вернуться в список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditOfficeAsync([FromForm] OfficeModel model)
        {
            return await SaveOffice(model, actionName: "OfficeList");
        }

        /// <summary>
        /// Сохранить нефтебазу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveOfficeAsync([FromForm] OfficeModel model)
        {
            return await SaveOffice(model);
        }

        /// <summary>
        /// Сохранить запись нефтебазы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveOffice(OfficeModel model, string actionName = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("Office", model);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена";
            var result = await _gasStationService.SaveOfficeAsync(model);

            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("Office", model);
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation",
                $"{actionVerb} запись {model.ObjectName}");

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return Redirect($"/Station/Office");
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/Station/Office/{result.Id}");
        }
        #endregion

        #region Laboratories

        [Route("Station/Laboratory")]
        public async Task<IActionResult> Laboratory()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase", "Открыт справочник нефтебаз.");
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);

            return View("LaboratoryList");
        }

        [HttpPost]
        public async Task<IActionResult> LoadLaboratoryList()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var person = await _masterService.GetPersonAsync(user.Id);
            int? personId = null;

            var stations = await GetLaboratoryListItems(personId);

            return Json(new { data = stations });
        }

        private async Task<IEnumerable<LaboratoryListItem>> GetLaboratoryListItems(int? personId = null)
        {
            IEnumerable<LaboratoryListItem> stations;
            if (personId != null)
            {
                stations = await _gasStationService.GetLaboratoryListAsync(personId.Value);
            }
            else
            {
                stations = await _gasStationService.GetLaboratoryListAsync();
            }

            return stations;
        }

        [Route("Station/Laboratory/{id}")]

        public async Task<IActionResult> GetLaboratoryAsync(int id)
        {
            var station = await _gasStationService.GetLaboratoryAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "fuelbase",
                $"Открыта запись {station.ObjectName}");

            return View("Laboratory", station);
        }

        [Route("Station/CreateLaboratory")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateLaboratoryAsync(string regions, string terrs)
        {
            var model = new LaboratoryModel();
            return View("Laboratory", model);
        }

        /// <summary>
        /// Сохранить нефтебазу и вернуться в список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditLaboratoryAsync([FromForm] LaboratoryModel model)
        {
            return await SaveLaboratory(model, actionName: "LaboratoryList");
        }

        /// <summary>
        /// Сохранить нефтебазу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SaveLaboratoryAsync([FromForm] LaboratoryModel model)
        {
            return await SaveLaboratory(model);
        }

        /// <summary>
        /// Сохранить запись нефтебазы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveLaboratory(LaboratoryModel model, string actionName = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("Laboratory", model);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена";
            var result = await _gasStationService.SaveLaboratoryAsync(model);

            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return View("Laboratory", model);
            }

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "gasstation",
                $"{actionVerb} запись {model.ObjectName}");

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return Redirect($"/Station/Laboratory");
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/Station/Laboratory/{result.Id}");
        }
        #endregion
    }
}