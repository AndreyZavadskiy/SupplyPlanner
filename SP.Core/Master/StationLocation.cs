using System.ComponentModel.DataAnnotations.Schema;

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
