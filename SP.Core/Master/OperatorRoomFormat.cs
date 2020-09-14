using System.ComponentModel.DataAnnotations.Schema;

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
