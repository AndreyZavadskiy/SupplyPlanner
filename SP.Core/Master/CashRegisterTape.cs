using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Тип кассовой термоленты
    /// </summary>
    [Table("CashRegisterTape", Schema = "dic")]
    public class CashRegisterTape : DictionaryItem
    {
    }
}
