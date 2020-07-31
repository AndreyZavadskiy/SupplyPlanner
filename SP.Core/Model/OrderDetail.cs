using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Text;

namespace SP.Core.Model
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID заказа
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// </summary>
        public int NomenclatureId { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int GasStationId { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }

        #region Navigation properties

        /// <summary>
        /// Заказ
        /// </summary>
        public Order Order { get; set; }
        /// <summary>
        /// Номенклатура
        /// </summary>
        public Nomenclature Nomenclature { get; set; }
        /// <summary>
        /// АЗС
        /// </summary>
        public GasStation GasStation { get; set; }

        #endregion
    }
}
