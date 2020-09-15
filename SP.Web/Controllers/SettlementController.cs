using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class SettlementController : BaseDictionaryController<CashboxLocation>
    {
        public SettlementController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Населенный пункт";
            ClassName = "Settlement";
        }
    }
}