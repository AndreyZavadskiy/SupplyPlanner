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
                        Person = p,
                        AppUser = u,
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
                    p => p.Person.AspNetUserId,
                    r => r.UserId,
                    (p, r) => new
                    {
                        p.Person,
                        p.AppUser,
                        r.RoleName
                    })
                .GroupJoin(_context.PersonTerritories,
                    p => p.Person.Id,
                    t => t.PersonId,
                    (p, t) => new
                    {
                        p.Person,
                        p.AppUser,
                        p.RoleName,
                        Terrtory = t
                    })
                .SelectMany(x => x.Terrtory.DefaultIfEmpty(),
                    (x, y) => new 
                    {
                        x.Person.Id,
                        x.Person.Code,
                        x.Person.LastName,
                        x.Person.FirstName,
                        x.Person.MiddleName,
                        x.AppUser.IsActive,
                        x.RoleName,
                        TerritoryName = y.RegionalStructure.Name
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
                    RoleDescription = string.Join(", ", z.Select(r => r.RoleName).Distinct()),
                    TerritoryDescription = string.Join(", ", z.Select(t => t.TerritoryName).Distinct()),
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

            var userTerritories = await _context.PersonTerritories
                .Where(x => x.PersonId == id)
                .Select(x => x.RegionalStructureId)
                .ToArrayAsync();

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
                RoleName = userRoles.FirstOrDefault(),
                Territories = string.Join(',', userTerritories)
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

            return await UpdateUserAsync(dbPerson, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertUserAsync(UserModel model)
        {
            var appUser = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true,
                IsActive = !model.Inactive,
                RegistrationDate = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentException("Пароль не может быть пустым.");
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

                    if (!string.IsNullOrWhiteSpace(model.Territories))
                    {
                        var terrList = model.Territories.Split(',');
                        foreach (var terrId in terrList)
                        {
                            var personTerritory = new PersonTerritory
                            {
                                Person = person,
                                RegionalStructureId = Convert.ToInt32(terrId)
                            };

                            await _context.PersonTerritories.AddAsync(personTerritory);
                        }
                    }

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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateUserAsync(Person person, UserModel model)
        {
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

                // PersonTerritory
                var dbTerritories = await _context.PersonTerritories
                    .Where(x => x.PersonId == model.Id)
                    .ToArrayAsync();
                var actualList = string.IsNullOrWhiteSpace(model.Territories)
                    ? new int[0]
                    : model.Territories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x));
                var listToInsert = actualList.Except(dbTerritories.Select(x => x.RegionalStructureId));
                foreach (var terr in listToInsert)
                {
                    var newPersonTerr = new PersonTerritory
                    {
                        PersonId = model.Id,
                        RegionalStructureId = terr
                    };

                    await _context.PersonTerritories.AddAsync(newPersonTerr);
                }

                var listToDelete = dbTerritories
                    .Where(x => !actualList.Any(y => x.RegionalStructureId == y));
                _context.PersonTerritories.RemoveRange(listToDelete);

                // AspNetUser
                appUser.UserName = model.UserName;
                appUser.Email = model.Email;
                appUser.IsActive = !model.Inactive;
                var updateUserResult = await _userManager.UpdateAsync(appUser);
                if (!updateUserResult.Succeeded)
                {
                    return (false, model.Id, updateUserResult.Errors.Select(e => e.Description));
                }

                // пароль
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var passResult = await _userManager.RemovePasswordAsync(appUser);
                    passResult = await _userManager.AddPasswordAsync(appUser, model.Password);
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
