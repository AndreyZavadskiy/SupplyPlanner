using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SP.Core.Model
{
    /// <summary>
    /// ТМЦ, состыкованные с Номенклатурой
    /// </summary>
    [Table("Inventory")]
    public class Inventory
    {
        /// <summary>
        /// ID ТМЦ
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование ТМЦ
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// null может быть в случае, если соответствение Номенклатуре еще не установлено,
        /// либо оно не вообще не будет задано (IsBlocked = true)
        /// </summary>
        public int? NomenclatureId { get; set; }
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
        /// Cоответствие Номенклатуре никогда не будет устанавливаться
        /// Позиция ТМЦ исключена из дальнейшей обработки
        /// </summary>
        public bool IsBlocked { get; set; }
        /// <summary>
        /// Дата актуализации
        /// </summary>
        public DateTime LastUpdate { get; set; }

        #region Navigation properties

        /// <summary>
        /// Номенклатура
        /// </summary>
        [ForeignKey("NomenclatureId")]
        public Nomenclature Nomenclature { get; set; }
        /// <summary>
        /// АЗС
        /// </summary>
        [ForeignKey("GasStationId")]
        public GasStation GasStation { get; set; }

        #endregion
    }
}
