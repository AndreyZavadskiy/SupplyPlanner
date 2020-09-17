using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Master;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IMasterService _masterService;
        private readonly ILogService _logService;
        private readonly IUserService _userService;

        public ReportController(IMasterService masterService, ILogService logService, IUserService userService)
        {
            _masterService = masterService;
            _logService = logService;
            _userService = userService;
        }

        public async Task<IActionResult> ActionReportAsync()
        {
            var users = await _userService.GetUserNameListAsync();
            var list = new SelectList(users, "Id", "Name").ToList();
            list.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["UserList"] = list;

            return View("ActionReport");
        }

        [HttpPost]
        public async Task<IActionResult> LoadActionList(int? user, DateTime? start, DateTime? end)
        {
            if (user == null && start == null && end == null)
            {
                var zeroItem = new ActionListItem{ Description = "Установите фильтры для отображения данных"};
                return Json(new { data = new[] { zeroItem } });
            }

            var list = await _logService.GetActionListAsync(user, start, end);
            return Json(new { data = list });
        }

        public async Task<IActionResult> BalanceReportAsync()
        {
            await LoadEssentialDictionaries();

            return View("BalanceReport");
        }

        [HttpPost]
        public async Task<IActionResult> LoadBalanceList(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            if (region == null && terr == null && station == null && group == null && nom == null && start == null && end == null)
            {
                var zeroItem = new
                {
                    groupName = string.Empty,
                    nomenclatureName = "Установите фильтры для отображения данных",
                    stationNumber = string.Empty,
                    actionDate = (DateTime?)null,
                    quantity = (decimal?)null
                };
                return Json(new { data = new[] { zeroItem } });
            }

            var data = await _logService.GetBalanceListAsync(region, terr, station, group, nom, start, end);
            var list = data.Select(x => new
            {
                groupName = x.NomenclatureGroupName,
                nomenclatureName = x.NomenclatureName,
                stationNumber = x.StationNumber,
                actionDate = x.Date,
                quantity = x.Quantity
            });
            return Json(new { data = list });
        }

        public async Task<IActionResult> ExceedPlanReportAsync()
        {
            await LoadEssentialDictionaries();

            return View("ExceedPlanReport");
        }

        [HttpPost]
        public async Task<IActionResult> LoadExceedPlanList(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            if (region == null && terr == null && station == null && group == null && nom == null && start == null && end == null)
            {
                var zeroItem = new
                {
                    groupName = string.Empty,
                    nomenclatureName = "Установите фильтры для отображения данных",
                    stationNumber = string.Empty,
                    actionDate = (DateTime?)null,
                    plan = (decimal?)null,
                    quantity = (decimal?)null
                };
                return Json(new { data = new[] { zeroItem } });
            }

            var data = await _logService.GetExceedPlanListAsync(region, terr, station, group, nom, start, end);
            var list = data.Select(x => new
            {
                groupName = x.NomenclatureGroupName,
                nomenclatureName = x.NomenclatureName,
                stationNumber = x.StationNumber,
                actionDate = x.Date,
                plan = x.Plan,
                quantity = x.Quantity
            });
            return Json(new { data = list });
        }

        public async Task<IActionResult> OrderReportAsync()
        {
            await LoadEssentialDictionaries();

            return View("OrderReport");
        }

        [HttpPost]
        public async Task<IActionResult> LoadOrderDetailList(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            if (region == null && terr == null && station == null && group == null && nom == null && start == null && end == null)
            {
                var zeroItem = new
                {
                    groupName = string.Empty,
                    nomenclatureName = "Установите фильтры для отображения данных",
                    stationNumber = string.Empty,
                    actionDate = (DateTime?)null,
                    quantity = (decimal?)null,
                    orderNumber = (int?)null
                };
                return Json(new { data = new[] { zeroItem } });
            }

            var data = await _logService.GetOrderListAsync(region, terr, station, group, nom, start, end);
            var list = data.Select(x => new
            {
                groupName = x.NomenclatureGroupName,
                nomenclatureName = x.NomenclatureName,
                stationNumber = x.StationNumber,
                actionDate = x.Date,
                quantity = x.Quantity,
                orderNumber = x.OrderNumber
            });
            return Json(new { data = list });
        }

        public async Task<IActionResult> ChangeReportAsync()
        {
            var users = await _userService.GetUserNameListAsync();
            var list = new SelectList(users, "Id", "Name").ToList();
            list.Insert(0, new SelectListItem("-- ВСЕ --", ""));
            ViewData["UserList"] = list;

            return View("ChangeReport");
        }

        [HttpPost]
        public async Task<IActionResult> LoadChangeList(int? user, DateTime? start, DateTime? end)
        {
            if (user == null && start == null && end == null)
            {
                var zeroItem = new ChangeListItem { OldValue = "Установите фильтры для отображения данных" };
                return Json(new { data = new[] { zeroItem } });
            }

            var list = await _logService.GetChangeListAsync(user, start, end);
            return Json(new { data = list });
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
    }
}
