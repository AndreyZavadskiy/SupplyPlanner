using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Core.Model;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IMasterService
    {
        Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryListItem;

        Task<IEnumerable<DictionaryListItem>> GetRegionListAsync();
        Task<RegionModel> GetRegionAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveRegionAsync(RegionModel model);
        Task<IEnumerable<TerritoryListItem>> GetTerritoryListAsync();
    }

    public class MasterService : IMasterService
    {
        private readonly ApplicationDbContext _context;

        public MasterService(ApplicationDbContext context)
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

        public async Task<IEnumerable<DictionaryListItem>> GetRegionListAsync()
        {
            var list = await _context.Regions.AsNoTracking()
                .Where(x => x.ParentId == null)
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.IsActive
                        ? x.Name
                        : $"{x.Name} (исключен)"
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<RegionModel> GetRegionAsync(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
            {
                return null;
            }

            var regionModel = new RegionModel
            {
                Id = region.Id,
                ParentId = region.ParentId,
                Name = region.Name,
                Inactive = !region.IsActive
            };

            return regionModel;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveRegionAsync(RegionModel model)
        {
            bool hasRegionWithSameName = await _context.Regions.AnyAsync(x => x.Id != model.Id && x.Name == model.Name);
            if (hasRegionWithSameName)
            {
                return (false, null, new[] {"Регион с таким наименованием уже существует. Нельзя создавать дубли."});
            }

            if (model.Id == 0)
            {
                return await CreateRegionAsync(model);
            }

            return await UpdateRegionAsync(model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> CreateRegionAsync(RegionModel model)
        {
            var region = new Region
            {
                Name = model.Name,
                ParentId = model.ParentId,
                IsActive = !model.Inactive
            };

            var errors = new List<string>();
            try
            {
                _context.Regions.Add(region);
                await _context.SaveChangesAsync();

                return (true, region.Id, null);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Ошибка создания региона в системе.");
            }

            return (false, null, errors);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateRegionAsync(RegionModel model)
        {
            var region = await _context.Regions.FindAsync(model.Id);
            if (region == null)
            {
                return (false, model.Id, new[] { "Регион не найден в базе данных." });
            }

            var errors = new List<string>();
            try
            {
                region.Name = model.Name;
                region.ParentId = model.ParentId;
                region.IsActive = !model.Inactive;
                _context.Regions.Update(region);
                await _context.SaveChangesAsync();

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
                errors.Add("Ошибка изменения региона в системе.");
            }

            return (false, null, errors);
        }

        public async Task<IEnumerable<TerritoryListItem>> GetTerritoryListAsync()
        {
            var list = await _context.Regions.AsNoTracking()
                .Where(r => r.ParentId == null)
                .Join(
                    _context.Regions.AsNoTracking(),
                    r => r.Id,
                    t => t.ParentId,
                    (r, t) => new TerritoryListItem
                    {
                        RegionId = r.Id,
                        RegionName = r.Name,
                        TerritoryId = t.Id,
                        TerritoryName = t.Name,
                        Active = t.IsActive ? "1" : "0"
                    })
                .ToArrayAsync();

            return list;
        }
    }
}
