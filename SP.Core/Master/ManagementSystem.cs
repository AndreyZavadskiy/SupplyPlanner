using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

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
