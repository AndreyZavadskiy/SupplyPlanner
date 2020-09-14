using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Система управления
    /// </summary>
    [Table("ManagementSystem", Schema = "dic")]
    public class ManagementSystem : DictionaryItem
    {
    }
}
