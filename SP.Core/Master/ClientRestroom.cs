using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

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
