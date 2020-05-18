using System.Collections.Generic;
using System.Threading.Tasks;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IDictionaryService
    {
        Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryListItem;

        Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync();
    }
}