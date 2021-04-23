using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class SegmentController : BaseDictionaryController<Segment>
    {
        public SegmentController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Сегмент";
            ClassName = "Segment";
        }
    }
}