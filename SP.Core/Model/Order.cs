using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
        public int Id { get; set; }
        /// <summary>
        /// Дата заказа
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Сотрудник, сделавший заказ
        /// </summary>
        public int PersonId { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        #endregion
    }
}
