using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ManagementSystemController : BaseDictionaryController<CashboxLocation>
    {
        public ManagementSystemController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Система управления";
            ClassName = "ManagementSystem";
        }
    }
}