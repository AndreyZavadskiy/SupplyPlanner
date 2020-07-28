using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Остатки ТМЦ в разрезе Номенклатуры
    /// </summary>
    [Table("NomenclatureBalance")]
    public class NomenclatureBalance
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
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
        /// Номенклатура
        /// </summary>
        [ForeignKey("NomenclatureId")]
        public Nomenclature Nomenclature { get; set; }

        #endregion
    }
}
