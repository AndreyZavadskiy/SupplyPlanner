using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Master;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ClientRestroomController : Controller
    {
        private readonly IMasterService _masterService;
        private readonly string _title = "Санузел для клиентов";
        private readonly string _className = "ClientRestroom";

        public ClientRestroomController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;
            return View("MasterDictionary");
        }

        [HttpPost]
        [Route("[controller]/LoadList")]
        public async Task<IActionResult> LoadListAsync()
        {
            var list = await _masterService.GetDictionaryListAsync<ClientRestroom>();

            return Json(new { data = list });
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            var model = await _masterService.GetDictionaryModelAsync<ClientRestroom>(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;

            return View("_MasterDictionaryEdit", model);
        }

        public IActionResult Create()
        {
            var model = new DictionaryModel();

            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;

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

            var result = await _masterService.SaveDictionaryAsync<ClientRestroom>(model);
            if (result.Success)
            {
                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }
    }
}