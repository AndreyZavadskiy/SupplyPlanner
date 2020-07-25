using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Площадь торгового зала
    /// </summary>
    [Table("TradingHallSize", Schema = "dic")]
    public class TradingHallSize : DictionaryItem
    {
    }
}
