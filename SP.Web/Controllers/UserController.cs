using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Data;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMasterService _masterService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppLogger _appLogger;

        public UserController(IUserService userService, IMasterService masterService, UserManager<ApplicationUser> userManager, IAppLogger appLogger)
        {
            _userService = userService;
            _masterService = masterService;
            _userManager = userManager;
            _appLogger = appLogger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "user", "Открыт справочник пользователей.");

            return View("Index");
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

            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "user",
                $"Cправочник пользователей. Открыта запись код {user.Code}, {user.LastName} {user.FirstName} {user.MiddleName}");

            return await PrepareUserViewAsync(user);
        }

        private async Task<IActionResult> PrepareUserViewAsync(UserModel user)
        {
            var roleList = await _userService.GetRolesAsync();
            ViewData["RoleList"] = new SelectList(roleList, "Id", "Name");
            if (string.IsNullOrWhiteSpace(user.Territories))
            {
                ViewData["TerritoryNames"] = string.Empty;
            }
            else
            {
                var territoryList = await _masterService.SelectTerritoryAsync(null);
                var selectedTerritories = user.Territories.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var nameList = territoryList
                    .Join(selectedTerritories,
                        t => t.Id.ToString(),
                        s => s,
                        (t, s) => t.Name);
                ViewData["TerritoryNames"] = string.Join(", ", nameList);
            }

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
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                if (model.Id == 0)
                {
                    ModelState.AddModelError("Password", "Не задан пароль пользователя");
                }
            }
            else
            {
                if (model.Password != model.PasswordRepeat)
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

            string actionVerb = model.Id == 0 ? "Создан" : "Изменен";
            var result = await _userService.SaveUserAsync(model);
            if (!result.Success)
            {
                string errorMessage = string.Join("<br/>", result.Errors);
                TempData["ActionMessage"] = $"Ошибки при сохранении записи:<br/>{errorMessage}";
                TempData["ActionMessageClass"] = "alert-danger";
                return await PrepareUserViewAsync(model);
            }

            string code = string.IsNullOrWhiteSpace(model.Code) ? model.Id.ToString() : model.Code;
            await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "user",
                $"Cправочник пользователей. {actionVerb} запись код {code}, {model.LastName} {model.FirstName} {model.MiddleName}");

            if (!string.IsNullOrWhiteSpace(actionName))
            {
                return RedirectToAction(actionName);
            }

            TempData["ActionMessage"] = "Запись сохранена в базе данных.";
            TempData["ActionMessageClass"] = "alert-info";

            return Redirect($"/User/{result.Id}");
        }

        public IActionResult TerritorySelection(string selected)
        {
            ViewData["SelectedTerritories"] = selected;
            return View("_TerritorySelection");
        }
    }
}