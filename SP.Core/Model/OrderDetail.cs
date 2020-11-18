using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// ID заказа
        /// </summary>
        public long OrderId { get; set; }
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
