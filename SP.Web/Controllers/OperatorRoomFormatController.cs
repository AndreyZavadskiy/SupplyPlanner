﻿using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class OperatorRoomFormatController : BaseDictionaryController<OperatorRoomFormat>
    {
        public OperatorRoomFormatController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Формат операторной";
            ClassName = "OperatorRoomFormat";
        }
    }
}