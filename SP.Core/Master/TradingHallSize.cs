using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

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
