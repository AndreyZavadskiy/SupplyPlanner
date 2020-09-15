using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Master;
using SP.Service.Models;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    public abstract class BaseDictionaryController<T> : Controller
        where T : DictionaryItem, new()
    {
        protected readonly IMasterService MasterService;
        protected readonly IAppLogger AppLogger;

        protected string Title;
        protected string ClassName;

        protected BaseDictionaryController(IMasterService masterService, IAppLogger appLogger)
        {
            MasterService = masterService;
            AppLogger = appLogger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            await LogActionAsync(User.Identity.Name, "dictionary", $"Открыт справочник {Title}.");

            ViewData["Title"] = Title;
            ViewData["ClassName"] = ClassName;
            return View("MasterDictionary");
        }

        [HttpPost]
        [Route("[controller]/LoadList")]
        public async Task<IActionResult> LoadListAsync()
        {
            var list = await MasterService.GetDictionaryListAsync<T>();

            return Json(new { data = list });
        }

        public IActionResult Create()
        {
            var model = new DictionaryModel();

            ViewData["Title"] = Title;
            ViewData["ClassName"] = ClassName;

            return View("_MasterDictionaryEdit", model);
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            var model = await MasterService.GetDictionaryModelAsync<T>(id);
            if (model == null)
            {
                return NotFound();
            }

            await LogActionAsync(User.Identity.Name, "dictionary",
                $"Cправочник {Title}. Открыта запись id {model.Id}, {model.Name}");

            ViewData["Title"] = Title;
            ViewData["ClassName"] = ClassName;

            return View("_MasterDictionaryEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([FromForm] DictionaryModel model)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(ms => ms.Errors)
                    .Select(e => e.ErrorMessage);
                errorMessage = string.Join(" ", errors);
                return Content(errorMessage);
            }

            string actionVerb = model.Id == 0 ? "Создана" : "Изменена";

            var result = await MasterService.SaveDictionaryAsync<T>(model);
            if (result.Success)
            {
                await LogActionAsync(User.Identity.Name, "dictionary",
                    $"Cправочник {Title}. {actionVerb} запись id {model.Id}, {model.Name}");

                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }

        protected async Task<bool> LogActionAsync(string name, string category, string description)
        {
            return await AppLogger.SaveActionAsync(name, DateTime.Now, category, description);
        }
    }
}
