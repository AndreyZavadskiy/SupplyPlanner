﻿using Microsoft.AspNetCore.Authorization;
using SP.Core.Master;
using SP.Service.Services;
using SP.Web.Utility;

namespace SP.Web.Controllers
{
    [Authorize]
    public class NomenclatureGroupController : BaseDictionaryController<CashboxLocation>
    {
        public NomenclatureGroupController(IMasterService masterService, IAppLogger appLogger) : base(masterService, appLogger)
        {
            Title = "Группы номенклатуры";
            ClassName = "NomenclatureGroup";
        }
    }
}