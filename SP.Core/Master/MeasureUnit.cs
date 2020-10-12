using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Единицы измерения
    /// </summary>
    [Table("MeasureUnit", Schema = "dic")]
    public class MeasureUnit : DictionaryItem
    {
    }
}
