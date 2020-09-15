using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class StationStatusController : BaseDictionaryController<CashboxLocation>
    {
        public StationStatusController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Статус";
            ClassName = "StationStatus";
        }
    }
}