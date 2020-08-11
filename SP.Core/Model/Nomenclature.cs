using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Master;

namespace SP.Core.Model
{
    /// <summary>
    /// Номенклатура товаров и материалов
    /// </summary>
    [Table("Nomenclature")]
    public class Nomenclature
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Код Петроникса
        /// </summary>
        [StringLength(20)]
        public string PetronicsCode { get; set; }
        /// <summary>
        /// Наименование Петроникса
        /// </summary>
        [StringLength(100)]
        public string PetronicsName { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public int MeasureUnitId { get; set; }
        /// <summary>
        /// Группа номенклатуры
        /// </summary>
        public int NomenclatureGroupId { get; set; }
        /// <summary>
        /// Срок полезного использования, месяцев
        /// </summary>
        public int UsefulLife { get; set; }
        /// <summary>
        /// Является активной (подлежит заказу)
        /// </summary>
        public bool IsActive { get; set; }

        #region Navigation properties

        [ForeignKey("MeasureUnitId")]
        public MeasureUnit MeasureUnit { get; set; }
        [ForeignKey("NomenclatureGroupId")]
        public NomenclatureGroup NomenclatureGroup { get; set; }

        /// <summary>
        /// Остатки ТМЦ по конкретной Номенклатуре
        /// </summary>
        public ICollection<Inventory> Inventories { get; set; }
        /// <summary>
        /// Остатки и потребности по Номенклатуре
        /// </summary>
        public ICollection<NomCalculation> NomCalculations { get; set; }

        #endregion
    }
}
