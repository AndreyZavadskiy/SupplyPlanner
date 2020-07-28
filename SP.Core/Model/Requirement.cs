using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Потребность Номенклатуры в разрезе АЗС
    /// </summary>
    [Table("Requirement")]
    public class Requirement
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
        /// Фикс.количество
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal FixedAmount { get; set; }
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

        #region Navigation properties

        [ForeignKey("NomenclatureId")]
        public Nomenclature Nomenclature { get; set; }
        [ForeignKey("GasStationId")]
        public GasStation GasStation { get; set; }

        #endregion
    }
}
