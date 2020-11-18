﻿using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class ServiceLevelController : BaseDictionaryController<ServiceLevel>
    {
        public ServiceLevelController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Кластер (уровень сервиса)";
            ClassName = "ServiceLevel";
        }
    }
}