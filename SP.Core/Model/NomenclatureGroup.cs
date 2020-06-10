using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SP.Core.Model
{
    /// <summary>
    /// Группа номенклатуры
    /// </summary>
    [Table("NomenclatureGroup")]
    public class NomenclatureGroup
    {
        /// <summary>
        /// ID группы номенклатуры
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование группы
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        #region Navigation properties

        /// <summary>
        /// Позиции Номенклатуры в этой группе
        /// </summary>
        public ICollection<Nomenclature> Nomenclatures { get; set; }

        #endregion
    }
}
