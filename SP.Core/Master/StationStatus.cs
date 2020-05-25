using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Master
{
    /// <summary>
    /// Статус АЗС
    /// </summary>
    [Table("StationStatus", Schema = "dic")]
    public class StationStatus : DictionaryItem
    {
    }
}
