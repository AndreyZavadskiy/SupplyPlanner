using System;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Enum;

namespace SP.Core.Model
{
    /// <summary>
    /// Заказы ТМЦ
    /// </summary>
    [Table("Order")]

    public class Order
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Дата заказа
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Сотрудник, сделавший заказ
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// Тип заказа
        /// </summary>
        public OrderType OrderType { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        #endregion
    }
}
