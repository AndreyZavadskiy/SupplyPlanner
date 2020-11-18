using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class TradingHallOperatingModeController : BaseDictionaryController<TradingHallOperatingMode>
    {
        public TradingHallOperatingModeController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Режим работы торгового зала";
            ClassName = "TradingHallOperatingMode";
        }
    }
}