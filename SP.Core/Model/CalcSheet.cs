using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Расчетный лист (остаток и потребность Номенклатуры по АЗС)
    /// </summary>
    [Table("CalcSheet")]
    public class CalcSheet
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// </summary>
        public int NomenclatureId { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int GasStationId { get; set; }
        /// <summary>
        /// Остаток
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Фикс.количество
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal? FixedAmount { get; set; }
        /// <summary>
        /// Формула расчета
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// Кратность партии
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal MultipleFactor { get; set; }
        /// <summary>
        /// Округление
        /// 1 - вниз, 2 - вверх, 3 - до ближайшего целого
        /// </summary>
        public int Rounding { get; set; }
        /// <summary>
        /// План потребности
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal Plan { get; set; }

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
