using System.Collections.Generic;
using System.Threading.Tasks;
using SP.Service.Models;

namespace SP.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListData>> GetUserListAsync();
    
        Task<UserModel> GetUserAsync(int id);

        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveUserAsync(UserModel model);

        Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync();
    }
}