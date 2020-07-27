using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.ViewModels
{
    /// <summary>
    /// Данные для объединения ТМЦ с Номенклатурой
    /// </summary>
    public class InventoryLinkViewModel
    {
        /// <summary>
        /// ID записей ТМЦ
        /// </summary>
        public int[] Inventories { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// </summary>
        public int Nomenclature { get; set; }
    }
}
