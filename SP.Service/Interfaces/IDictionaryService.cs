using System.Collections.Generic;
using System.Threading.Tasks;
using SP.Service.Models;

namespace SP.Service.Interfaces
{
    public interface IDictionaryService
    {
        Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryListItem;
    }
}