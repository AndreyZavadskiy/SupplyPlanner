using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Enum;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.Controllers
{
    [Authorize]
    public class NomenclatureGroupController : BaseDictionaryController<NomenclatureGroup>
    {
        public NomenclatureGroupController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Группы номенклатуры";
            ClassName = "NomenclatureGroup";
        }

        public async Task<IActionResult> ListByOjectTypeAsync(ObjectType type)
        {
            var groups = await MasterService.GetDictionaryListAsync<NomenclatureGroup>();
            var groupsByObjectType = await MasterService.GetNomenclatureGroupsAsync(type);
            var list = groups.Where(x => groupsByObjectType.Contains(x.Id)).ToArray();

            return Json(new { data = list });
        }
    }
}