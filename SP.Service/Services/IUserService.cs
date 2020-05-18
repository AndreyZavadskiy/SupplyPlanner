using System.Collections.Generic;
using System.Threading.Tasks;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserListData>> GetUserListAsync();
        Task<UserModel> GetUserAsync(int id);
    }
}