using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SP.Service.Models;

namespace SP.Web.ViewModels
{
    /// <summary>
    /// Территория
    /// </summary>
    public class TerritoryViewModel
    {
        /// <summary>
        /// Регион
        /// </summary>
        public RegionModel Region { get; set; }
        /// <summary>
        /// Территория
        /// </summary>
        public RegionModel Territory { get; set; }
    }
}
