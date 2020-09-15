using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Core.History;
using SP.Core.Model;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface ILogService
    {
        Task<bool> SaveActionAsync(string userId, DateTime date, string category, string description);
        Task<IEnumerable<ActionListItem>> GetActionListAsync(int? user, DateTime? start, DateTime? end);
    }

    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveActionAsync(string id, DateTime date, string category, string description)
        {
            var person = await _context.Persons.FirstOrDefaultAsync(x => x.AspNetUserId == id);
            if (person == null)
            {
                return false;
            }

            var logEntry = new ActionLog
            {
                PersonId = person.Id,
                ActionDate = date,
                Category = category,
                Description = description
            };

            await _context.ActionLogs.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ActionListItem>> GetActionListAsync(int? user, DateTime? start, DateTime? end)
        {
            var query = _context.ActionLogs.AsNoTracking();
            if (user != null)
            {
                query = query.Where(x => x.PersonId == user);
            }
            if (start != null)
            {
                query = query.Where(x => start <= x.ActionDate);
            }
            if (end != null)
            {
                end = end.Value.Date.AddDays(1);
                query = query.Where(x => x.ActionDate < end);
            }

            var result = await query
                .Include(x => x.Person)
                .Select(x => new
                {
                    x.Id,
                    x.Person.LastName,
                    x.Person.FirstName,
                    x.Person.MiddleName,
                    x.ActionDate,
                    x.Description
                })
                .OrderBy(z => z.ActionDate)
                .Take(5000)
                .ToArrayAsync();

            var list = result.Select(x => new ActionListItem
            {
                Id = x.Id,
                UserName = Person.ConcatenateFio(x.LastName, x.FirstName, x.MiddleName),
                ActionDate = x.ActionDate,
                Description = x.Description
            });

            return list;
        }
    }
}
 