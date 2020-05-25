using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Master
{
    /// <summary>
    /// Месторасположение фактическое
    /// </summary>
    [Table("StationLocation", Schema = "dic")]
    public class StationLocation : DictionaryItem
    {
    }
}
