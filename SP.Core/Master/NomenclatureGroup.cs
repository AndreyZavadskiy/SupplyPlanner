using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Master
{
    /// <summary>
    /// Группа номенклатуры
    /// </summary>
    [Table("NomenclatureGroup", Schema = "dic")]
    public class NomenclatureGroup : DictionaryItem
    {
        #region Navigation properties

        /// <summary>
        /// Позиции Номенклатуры в этой группе
        /// </summary>
        public ICollection<Nomenclature> Nomenclatures { get; set; }

        #endregion
    }
}
