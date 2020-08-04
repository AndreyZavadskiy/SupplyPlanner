using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.ViewModels
{
    /// <summary>
    /// Количество заказа позиции
    /// </summary>
    public class OrderQuantityViewModel
    {
        /// <summary>
        /// ID позиции остатка Номенклатуры
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
