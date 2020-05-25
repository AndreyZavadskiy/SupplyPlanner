using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Master
{
    /// <summary>
    /// Кластер (уровень сервиса)
    /// </summary>
    [Table("ServiceLevel", Schema = "dic")]
    public class ServiceLevel : DictionaryItem
    {
    }
}
