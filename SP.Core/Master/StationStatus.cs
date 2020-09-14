using System.ComponentModel.DataAnnotations.Schema;

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
