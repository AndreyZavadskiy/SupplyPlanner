using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SP.Web.Models;

namespace SP.Web.Controllers
{
    public class UserController : Controller
    {
        public class UserListInfo
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string FullName { get; set; }
            public string RoleDescription { get; set; }
            public string TerritoryDescription { get; set; }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoadData()
        {
            var row1 = new UserListInfo
            {
                Id = "648104d1-b0c3-4bd9-aab4-523329645a9c",
                Code = "1",
                FullName = "Иванов Иван Иванович",
                RoleDescription = string.Empty,
                TerritoryDescription = string.Empty
            };
            var row2 = new UserListInfo
            {
                Id = "cf6a1bf6-d8af-4973-9602-4ccd9994c148",
                Code = "2",
                FullName = "Петров Петр Петрович",
                RoleDescription = "Администратор",
                TerritoryDescription = string.Empty
            };

            var result = new List<UserListInfo>
            {
                row1,
                row2
            };

            return Json(new { data = result });
        }
    }
}