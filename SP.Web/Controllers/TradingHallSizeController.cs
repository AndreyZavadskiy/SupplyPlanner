using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class TradingHallSizeController : BaseDictionaryController<CashboxLocation>
    {
        public TradingHallSizeController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Размер торгового зала";
            ClassName = "TradingHallSize";
        }
    }
}