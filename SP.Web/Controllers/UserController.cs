using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Data;
using SP.Service.Interfaces;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Models;

namespace SP.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDictionaryService _dictionaryService;

        public UserController(IUserService userService, IDictionaryService dictionaryService)
        {
            _userService = userService;
            _dictionaryService = dictionaryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Получить список всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoadListAsync()
        {
            var userList = await _userService.GetUserListAsync();

            return Json(new { data = userList });
        }

        /// <summary>
        /// Получить данные по конкретному пользовтелю
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("User/{id}")]
        public async Task<IActionResult> EditAsync(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roleList = await _userService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roleList, "Id", "Name");

            return View("User", user);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] UserModel model)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Save([FromForm] UserModel model)
        {
            throw new NotImplementedException();
        }

    }
}