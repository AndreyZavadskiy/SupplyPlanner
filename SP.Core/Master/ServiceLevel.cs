using System.ComponentModel.DataAnnotations.Schema;

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
