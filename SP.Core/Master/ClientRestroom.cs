using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Сан.узел для клиентов
    /// </summary>
    [Table("ClientRestroom", Schema = "dic")]
    public class ClientRestroom : DictionaryItem
    {
    }
}
