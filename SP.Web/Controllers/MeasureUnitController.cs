using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class MeasureUnitController : BaseDictionaryController<MeasureUnit>
    {
        public MeasureUnitController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Единицы измерения";
            ClassName = "MeasureUnit";
        }
    }
}