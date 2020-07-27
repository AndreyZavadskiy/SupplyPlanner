using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Режим работы торгового зала
    /// </summary>
    [Table("TradingHallOperatingMode", Schema = "dic")]
    public class TradingHallOperatingMode : DictionaryItem
    {
    }
}
