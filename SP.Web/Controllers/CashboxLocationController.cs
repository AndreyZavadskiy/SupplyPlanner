using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class CashboxLocationController : BaseDictionaryController<CashboxLocation>
    {
        public CashboxLocationController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Расчетно-кассовый узел";
            ClassName = "CashboxLocation";
        }
    }
}