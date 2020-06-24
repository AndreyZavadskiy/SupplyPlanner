using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Data;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMasterService _masterService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService, IMasterService masterService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _masterService = masterService;
            _userManager = userManager;
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

        [Route("[controller]/Create")]
        public async Task<IActionResult> CreateAsync()
        {
            var newUser = new UserModel
            {
                Id = 0,
                Code = string.Empty,
                RegistrationDate = DateTime.Now
            };

            ViewData["IsCreating"] = true;

            return await PrepareUserViewAsync(newUser);
        }

        /// <summary>
        /// Вывести карточку пользователя для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("[controller]/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return await PrepareUserViewAsync(user);
        }

        private async Task<IActionResult> PrepareUserViewAsync(UserModel user)
        {
            var roleList = await _userService.GetRolesAsync();
            ViewData["RoleList"] = new SelectList(roleList, "Id", "Name");

            return View("User", user);
        }

        /// <summary>
        /// Сохранить карточку пользователя и перейти в список
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditAsync([FromForm] UserModel model)
        {
            return await SaveUser(model, actionName: "Index");
        }

        /// <summary>
        /// Сохранить карточку пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveAsync([FromForm] UserModel model)
        {
            return await SaveUser(model);
        }

        private async Task<IActionResult> SaveUser(UserModel model, string actionName = null)
        {
            if (model.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Не задан пароль пользователя");
                }
                else if (model.Password != model.PasswordRepeat)
                {
                    ModelState.AddModelError("Password", "Пароли не совпадают");
                }
            }

            if (!ModelState.IsValid)
            {
                TempData["ActionMessage"] = "Проверьте правильность заполнения полей.";
                TempData["ActionMessageClass"] = "alert-danger";
                if (model.Id == 0)
                {
                    ViewData["IsCreating"] = true;
                }

                return await PrepareUserViewAsync(model);
            }

            var result = await _userService.SaveUserAsync(model);
            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareUserViewAsync(model);
            }

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return RedirectToAction(actionName);
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/User/{result.Id}");
        }
    }
}