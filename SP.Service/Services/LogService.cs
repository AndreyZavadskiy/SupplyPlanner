using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Core.History;
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

        public Task<IEnumerable<ActionListItem>> GetActionListAsync(int? user, DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }
    }
}
 