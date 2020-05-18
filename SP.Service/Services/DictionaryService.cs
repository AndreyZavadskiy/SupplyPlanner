using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public class DictionaryService : IDictionaryService
    {
        private readonly ApplicationDbContext _context;

        public DictionaryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryListItem
        {
            var list = await _context.Set<T>().AsNoTracking()
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<IEnumerable<DictionaryListItem<string>>> GetRolesAsync()
        {
            var list = await _context.Roles.AsNoTracking()
                .Select(x => new DictionaryListItem<string>
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            return list;
        }
    }
}
