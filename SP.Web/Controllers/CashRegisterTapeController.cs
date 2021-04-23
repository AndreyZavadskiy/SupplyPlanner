using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class CashRegisterTapeController : BaseDictionaryController<CashRegisterTape>
    {
        public CashRegisterTapeController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Вид термоленты";
            ClassName = "CashRegisterTape";
        }
    }
}