using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly ILogService _logService;
        private readonly IUserService _userService;

        public ReportController( ILogService logService, IUserService userService)
        {
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
        public async Task<IEnumerable<ActionListItem>> LoadActionList(int? user, DateTime? start, DateTime? end)
        {
            var list = await _logService.GetActionListAsync(user, start, end);
            return list;
        }
    }
}
