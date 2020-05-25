using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Master
{
    /// <summary>
    /// Формат операторной
    /// </summary>
    [Table("OperatorRoomFormat", Schema = "dic")]
    public class OperatorRoomFormat : DictionaryItem
    {
    }
}
