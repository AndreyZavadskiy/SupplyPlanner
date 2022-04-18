using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SP.Web.Utility
{
    public static class DictionaryUtility
    {
        public static List<SelectListItem> GetObjectTypeList()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem("АЗС", ((int)Core.Enum.ObjectType.GasStation).ToString()),
                new SelectListItem("Нефтебазы", ((int)Core.Enum.ObjectType.FuelBase).ToString()),
                new SelectListItem("Офисы", ((int)Core.Enum.ObjectType.Office).ToString()),
                new SelectListItem("Лаборатории", ((int)Core.Enum.ObjectType.Laboratory).ToString()),
            };

            return list;
        }
    }
}
