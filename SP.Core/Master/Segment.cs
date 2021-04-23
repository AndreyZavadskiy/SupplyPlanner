using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Сегмент
    /// </summary>
    [Table("Segment", Schema = "dic")]
    public class Segment : DictionaryItem
    {
    }
}
