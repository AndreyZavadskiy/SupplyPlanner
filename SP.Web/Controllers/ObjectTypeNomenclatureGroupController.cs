using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Core.Enum;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.Controllers
{
    public class ObjectTypeNomenclatureGroupController : Controller
    {
        private readonly IMasterService _masterService;
        private readonly IAppLogger _appLogger;

        public ObjectTypeNomenclatureGroupController(IMasterService masterService, IAppLogger appLogger)
        {
            _masterService = masterService;
            _appLogger = appLogger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewData["ObjectTypeList"] = DictionaryUtility.GetObjectTypeList();

            var nomenclatureGroups = await _masterService.GetDictionaryListAsync<NomenclatureGroup>();
            var groupList = new SelectList(nomenclatureGroups, "Id", "Name").ToList();
            ViewData["NomenclatureGroupList"] = groupList;

            return View();
        }

        public async Task<IActionResult> LoadGroupsAsync(ObjectType type)
        {
            var groupIdList = await _masterService.GetNomenclatureGroupsAsync(type);
            return Json(groupIdList);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAsync(ObjectType objectType, IEnumerable<int> nomenclatureGroups)
        {
            if (objectType == 0)
                return Content("Некорректный тип объекта");

            var result = await _masterService.SaveNomenclatureGroupsAsync(objectType, nomenclatureGroups);
            if (result.Success)
            {
                await _appLogger.SaveActionAsync(User.Identity.Name, DateTime.Now, "nomenclaturegroup",
                    $"Изменен список групп номенклатуры по типу объекта {objectType}");

                return null;
            }

            string errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }
    }
}
