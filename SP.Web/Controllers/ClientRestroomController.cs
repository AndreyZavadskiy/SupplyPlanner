using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ClientRestroomController : BaseDictionaryController<CashboxLocation>
    {
        public ClientRestroomController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Санузел для клиентов";
            ClassName = "ClientRestroom";
        }
    }
}