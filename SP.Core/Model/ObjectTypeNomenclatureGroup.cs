using SP.Core.Enum;
using SP.Core.Master;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Номенклатура товаров и материалов
    /// </summary>
    [Table("ObjectTypeNomenclatureGroup")]
    public class ObjectTypeNomenclatureGroup
    {
        public int Id { get; set; }
        /// <summary>
        ///  Тип объекта сети
        /// </summary>
        public ObjectType ObjectType { get; set; }
        /// <summary>
        /// ID группы номеклатуры
        /// </summary>
        public int NomenclatureGroupId { get; set; }


        #region Navigation properties

        [ForeignKey("NomenclatureGroupId")]
        public NomenclatureGroup NomenclatureGroup { get; set; }

        #endregion
    }
}
