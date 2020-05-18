using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Data;
using SP.Service.Interfaces;
using SP.Service.Models;

namespace SP.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserListData>> GetUserListAsync()
        {
            // собираем данные из таблиц: Person, AspNetUser - AspNetUserRoles - AspNetRoles
            var persons = await _context.Persons.AsNoTracking()
                .Join(_context.Users.AsNoTracking(),
                    p => p.AspNetUserId,
                    u => u.Id,
                    (p, u) => p)
                .Join(
                    _context.UserRoles.AsNoTracking()
                        .Join(_context.Roles.AsNoTracking(),
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => new
                            {
                                UserId = ur.UserId,
                                RoleName = r.FriendlyName
                            }),
                    p => p.AspNetUserId,
                    r => r.UserId,
                    (p, r) => new
                    {
                        Id = p.Id,
                        LastName = p.LastName,
                        FirstName = p.FirstName,
                        MiddleName = p.MiddleName,
                        RoleName = r.RoleName
                    })
                .ToArrayAsync();

            var result = persons
                .GroupBy(x => new 
                {
                    x.Id, 
                    x.LastName,
                    x.FirstName,
                    x.MiddleName
                })
                .Select(z => new UserListData
                {
                    Id = z.Key.Id.ToString(),
                    Code = z.Key.Id.ToString(),
                    FullName = $"{z.Key.LastName} {z.Key.FirstName} {z.Key.MiddleName}",
                    RoleDescription = string.Join(", ", z.Select(r => r.RoleName))
                })
                .ToArray();

            return result;
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return null;
            }

            string aspUserId = person.AspNetUserId;
            var user = await _context.Users.FindAsync(aspUserId);
            if (user == null)
            {
                return null;
            }

            var userRole = _context.UserRoles
                .FirstOrDefault(x => x.UserId == aspUserId)
                ?.RoleId;

            var model = new UserModel
            {
                Id = person.Id,
                LastName = person.LastName,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                UserName = user.UserName,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate,
                Inactive = !user.IsActive,
                RoleId = userRole
            };

            return model;
        }

        public async Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync()
        {
            var list = await _context.Roles.AsNoTracking()
                .Select(x => new DictionaryListItem<string>
                {
                    Id = x.Id,
                    Name = x.FriendlyName
                })
                .ToArrayAsync();

            return list;
        }
    }
}
