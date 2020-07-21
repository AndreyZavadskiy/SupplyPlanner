using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SP.Core.Master;

namespace SP.Core.Model
{
    /// <summary>
    /// Выгрузка ТМЦ в разрезе АЗС
    /// </summary>
    [Table("Inventory", Schema = "stage")]
    public class StageInventory
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код ТМЦ
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// Наименование ТМЦ
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int GasStationId { get; set; }
        /// <summary>
        /// ID единицы измерения
        /// </summary>
        public int MeasureUnitId { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Дата актуализации
        /// </summary>
        public DateTime LastUpdate { get; set; }

        #region Navigation properties

        /// <summary>
        /// АЗС
        /// </summary>
        [ForeignKey("GasStationId")]
        public GasStation GasStation { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        [ForeignKey("MeasureUnitId")]
        public MeasureUnit MeasureUnit { get; set; }

        #endregion
    }
}
