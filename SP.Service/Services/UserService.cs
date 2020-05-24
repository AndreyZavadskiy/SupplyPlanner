using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SP.Core.Model;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserListItem>> GetUserListAsync();
        Task<UserModel> GetUserAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveUserAsync(UserModel model);
        Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync();
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserListItem>> GetUserListAsync()
        {
            // собираем данные из таблиц: Person, AspNetUser - AspNetUserRoles - AspNetRoles
            var persons = await _context.Persons.AsNoTracking()
                .Join(_context.Users.AsNoTracking(),
                    p => p.AspNetUserId,
                    u => u.Id,
                    (p, u) => new
                    {
                        p.Id,
                        p.Code,
                        p.AspNetUserId,
                        p.LastName,
                        p.FirstName,
                        p.MiddleName,
                        u.IsActive
                    })
                .Join(
                    _context.UserRoles.AsNoTracking()
                        .Join(_context.Roles.AsNoTracking(),
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => new
                            {
                                ur.UserId,
                                RoleName = r.FriendlyName
                            }),
                    p => p.AspNetUserId,
                    r => r.UserId,
                    (p, r) => new
                    {
                        p.Id,
                        p.Code,
                        p.LastName,
                        p.FirstName,
                        p.MiddleName,
                        r.RoleName,
                        p.IsActive
                    })
                .ToArrayAsync();

            var result = persons
                .GroupBy(x => new 
                {
                    x.Id, 
                    x.Code,
                    x.LastName,
                    x.FirstName,
                    x.MiddleName,
                    x.IsActive
                })
                .Select(z => new UserListItem
                {
                    Id = z.Key.Id,
                    Code = string.IsNullOrWhiteSpace(z.Key.Code) ? z.Key.Id.ToString() : z.Key.Code,
                    FullName = $"{z.Key.LastName} {z.Key.FirstName} {z.Key.MiddleName}",
                    RoleDescription = string.Join(", ", z.Select(r => r.RoleName)),
                    Active = z.Key.IsActive ? "1" : "0"
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

            var appUser = await _userManager.FindByIdAsync(aspUserId);
            if (user == null)
            {
                throw new ApplicationException("Пользователь не найден в системе авторизации");
            }

            var userRoles = await _userManager.GetRolesAsync(appUser);

            var model = new UserModel
            {
                Id = person.Id,
                Code = string.IsNullOrWhiteSpace(person.Code) ? person.Id.ToString() : person.Code,
                LastName = person.LastName,
                FirstName = person.FirstName,
                MiddleName = person.MiddleName,
                UserName = user.UserName,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate,
                Inactive = !user.IsActive,
                RoleName = userRoles.FirstOrDefault()
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveUserAsync(UserModel model)
        {
            var dbPerson = await _context.Persons.FindAsync(model.Id);
            if (dbPerson == null)
            {
                return await InsertUserAsync(model);
            }

            return await UpdateUserAsync(model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertUserAsync(UserModel model)
        {
            var appUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                IsActive = !model.Inactive,
                RegistrationDate = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = "Pa$$w0rd";
            }

            // создаем ApplicationUser в ASP.Net Identity
            var createUserResult = await _userManager.CreateAsync(appUser, model.Password);
            if (!createUserResult.Succeeded)
            {
                var createUserErrors = createUserResult.Errors
                    .Select(x => x.Description)
                    .ToArray();
                return (false, null, createUserErrors);
            }

            // добавляем роли
            var addRoleResult = await _userManager.AddToRoleAsync(appUser, model.RoleName);

            // создаем Person
            var errors = new List<string>();
            if (addRoleResult.Succeeded)
            {
                try
                {
                    var person = new Person
                    {
                        AspNetUserId = appUser.Id,
                        Code = model.Code,
                        LastName = model.LastName,
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName
                    };

                    _context.Persons.Add(person);
                    await _context.SaveChangesAsync();
                    //_logger.LogInformation("User created a new account with password.");

                    return (true, person.Id, null);
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex);
                    errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    errors.Add("Ошибка создания персоны в системе.");
                }
            }

            // если персона не создана, удаляем пользователя
            var deleteUserResult = await _userManager.DeleteAsync(appUser);
            if (deleteUserResult.Succeeded)
            {
                return (false, null, errors);
            }

            foreach (var e in deleteUserResult.Errors)
            {
                errors.Add(e.Description);
            }

            return (false, null, errors);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateUserAsync(UserModel model)
        {
            var person = await _context.Persons.FindAsync(model.Id);
            if (person == null)
            {
                return (false, model.Id, new[] {"Пользователь не найден в базе данных."});
            }

            var appUser = await _userManager.FindByIdAsync(person.AspNetUserId);
            if (appUser == null)
            {
                return (false, model.Id, new[] { "Пользователь не найден в системе авторизации." });
            }

            var errors = new List<string>();

            try
            {
                // Person
                person.LastName = model.LastName;
                person.FirstName = model.FirstName;
                person.MiddleName = model.MiddleName;
                _context.Persons.Update(person);
                await _context.SaveChangesAsync();

                // AspNetUser
                appUser.UserName = model.UserName;
                appUser.Email = model.Email;
                appUser.IsActive = !model.Inactive;
                var updateUserResult = await _userManager.UpdateAsync(appUser);
                if (!updateUserResult.Succeeded)
                {
                    return (false, model.Id, updateUserResult.Errors.Select(e => e.Description));
                }

                // роли
                var currentRoles = await _userManager.GetRolesAsync(appUser);
                if (!currentRoles.Contains(model.RoleName))
                {
                    await _userManager.RemoveFromRolesAsync(appUser, currentRoles);
                    await _userManager.AddToRoleAsync(appUser, model.RoleName);
                }

                return (true, model.Id, null);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Ошибка сохранения персоны в системе.");
            }

            return (false, model.Id, errors);
        }

        public async Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync()
        {
            var list = await _context.Roles.AsNoTracking()
                .Select(x => new DictionaryListItem<string>
                {
                    Id = x.Name,
                    Name = x.FriendlyName
                })
                .ToArrayAsync();

            return list;
        }
    }
}
