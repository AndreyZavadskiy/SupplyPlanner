using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

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
