﻿using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class StationLocationController : BaseDictionaryController<CashboxLocation>
    {
        public StationLocationController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Месторасположение";
            ClassName = "StationLocation";
        }
    }
}