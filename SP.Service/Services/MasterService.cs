﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Core.Enum;
using SP.Core.Master;
using SP.Core.Model;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IMasterService
    {
        Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryItem;

        Task<DictionaryModel> GetDictionaryModelAsync<T>(int id)
            where T : DictionaryItem;

        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveDictionaryAsync<T>(DictionaryModel model)
            where T : DictionaryItem, new();

        Task<RegionalStructureModel> GetRegionalStructureItemAsync(int id);
        Task<IEnumerable<DictionaryListItem>> SelectRegionAsync();
        Task<IEnumerable<DictionaryListItem>> SelectRegionAsync(int personId);
        Task<IEnumerable<DictionaryListItem>> SelectTerritoryAsync(int[] regions);
        Task<IEnumerable<DictionaryListItem>> SelectTerritoryAsync(int[] regions, int personId);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveRegionAsync(RegionalStructureModel model);
        Task<IEnumerable<TerritoryListItem>> GetTerritoryListAsync();

        Task<Person> GetPersonAsync(string aspNetUserId);

        Task<IEnumerable<DictionaryListItem>> GetNomenclatureGroupForShortUse();

        Task<IEnumerable<int>> GetNomenclatureGroupsAsync(ObjectType objectType);
        Task<(bool Success, IEnumerable<string> Errors)> SaveNomenclatureGroupsAsync(ObjectType objectType, IEnumerable<int> groupIdList);
    }

    public class MasterService : IMasterService
    {
        private readonly ApplicationDbContext _context;

        public MasterService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Общие справочники

        public async Task<IEnumerable<DictionaryListItem>> GetDictionaryListAsync<T>()
            where T : DictionaryItem
        {
            var list = await _context.Set<T>().AsNoTracking()
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.IsActive ? "1" : "0"
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<DictionaryModel> GetDictionaryModelAsync<T>(int id)
            where T : DictionaryItem
        {
            var item = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return null;
            }

            var result = new DictionaryModel
            {
                Id = item.Id,
                Name = item.Name,
                Inactive = !item.IsActive
            };

            return result;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveDictionaryAsync<T>(DictionaryModel model)
            where T : DictionaryItem, new()
        {
            var dbItem = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == model.Id);

            if (dbItem == null)
            {
                T newItem = new T
                {
                    Id = model.Id,
                    Name = model.Name,
                    IsActive = !model.Inactive
                };

                await _context.Set<T>().AddAsync(newItem);
            }
            else if (dbItem.Name != model.Name || dbItem.IsActive != model.Inactive)
            {
                dbItem.Name = model.Name;
                dbItem.IsActive = !model.Inactive;
                _context.Set<T>().Update(dbItem);
            }

            var errors = new List<string>();
            try
            {
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
                errors.Add("Ошибка изменения записи справочника в системе.");
            }

            return (false, null, errors);
        }

        #endregion

        #region Регионы и территории

        public async Task<RegionalStructureModel> GetRegionalStructureItemAsync(int id)
        {
            var region = await _context.RegionStructure.FindAsync(id);
            if (region == null)
            {
                return null;
            }

            var regionModel = new RegionalStructureModel
            {
                Id = region.Id,
                ParentId = region.ParentId,
                Name = region.Name,
                Inactive = !region.IsActive
            };

            return regionModel;
        }

        public async Task<IEnumerable<DictionaryListItem>> SelectRegionAsync()
        {
            var list = await _context.RegionStructure.AsNoTracking()
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

        public async Task<IEnumerable<DictionaryListItem>> SelectRegionAsync(int personId)
        {
            var territories = await _context.PersonTerritories
                .Include(x => x.RegionalStructure)
                .Where(x => x.PersonId == personId)
                .Select(x => x.RegionalStructure.ParentId)
                .Distinct()
                .ToArrayAsync();

            var list = await _context.RegionStructure.AsNoTracking()
                .Where(x => x.ParentId == null && territories.Contains(x.Id))
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

        public async Task<IEnumerable<DictionaryListItem>> SelectTerritoryAsync(int[] regions)
        {
            if (regions == null)
            {
                var fullList = await _context.RegionStructure.AsNoTracking()
                    .Where(x => x.ParentId.HasValue )
                    .Select(x => new DictionaryListItem
                    {
                        Id = x.Id,
                        Name = x.IsActive
                            ? x.Name
                            : $"{x.Name} (исключен)"
                    })
                    .ToArrayAsync();
                
                return fullList;
            }

            var list = await _context.RegionStructure.AsNoTracking()
                .Where(x => x.ParentId != null && regions.Contains(x.ParentId.Value))
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

        public async Task<IEnumerable<DictionaryListItem>> SelectTerritoryAsync(int[] regions, int personId)
        {
            var territories = await _context.PersonTerritories
                .Where(x => x.PersonId == personId)
                .Select(x => x.RegionalStructureId)
                .Distinct()
                .ToArrayAsync();

            if (regions == null)
            {
                var fullList = await _context.RegionStructure.AsNoTracking()
                    .Where(x => x.ParentId.HasValue && territories.Contains(x.Id))
                    .Select(x => new DictionaryListItem
                    {
                        Id = x.Id,
                        Name = x.IsActive
                            ? x.Name
                            : $"{x.Name} (исключен)"
                    })
                    .ToArrayAsync();

                return fullList;
            }

            var list = await _context.RegionStructure.AsNoTracking()
                .Where(x => x.ParentId.HasValue && regions.Contains(x.ParentId.Value)
                    && territories.Contains(x.Id))
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

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveRegionAsync(RegionalStructureModel model)
        {
            bool hasRegionWithSameName = await _context.RegionStructure.AnyAsync(x => x.Id != model.Id && x.Name == model.Name);
            if (hasRegionWithSameName)
            {
                return (false, null, new[] { "Регион с таким наименованием уже существует. Нельзя создавать дубли." });
            }

            if (model.Id == 0)
            {
                return await CreateRegionAsync(model);
            }

            return await UpdateRegionAsync(model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> CreateRegionAsync(RegionalStructureModel model)
        {
            var region = new RegionalStructure
            {
                Name = model.Name,
                ParentId = model.ParentId,
                IsActive = !model.Inactive
            };

            var errors = new List<string>();
            try
            {
                _context.RegionStructure.Add(region);
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateRegionAsync(RegionalStructureModel model)
        {
            var region = await _context.RegionStructure.FindAsync(model.Id);
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
                _context.RegionStructure.Update(region);
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
            var list = await _context.RegionStructure.AsNoTracking()
                .Where(r => r.ParentId == null)
                .Join(
                    _context.RegionStructure.AsNoTracking(),
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

        #endregion

        public async Task<Person> GetPersonAsync(string aspNetUserId)
        {
            var person = await _context.Persons
                .FirstOrDefaultAsync(x => x.AspNetUserId == aspNetUserId);
            if (person == null)
            {
                throw new ApplicationException($"Не найден пользователь с id {aspNetUserId}");
            }

            return person;
        }

        public async Task<IEnumerable<DictionaryListItem>> GetNomenclatureGroupForShortUse()
        {
            string sqlStatement = @"
                SELECT DISTINCT ng.*
                FROM dic.""NomenclatureGroup"" ng
                WHERE EXISTS (
                    SELECT *
                    FROM public.""Nomenclature"" n
                    WHERE n.""UsefulLife"" <= 12
                    )
                ";
            var dbResult = _context.NomenclatureGroups.FromSqlRaw(sqlStatement);
            var result = dbResult
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.IsActive ? "1" : "0"
                })
                .OrderBy(x => x.Name);

            return result;
        }

        public async Task<IEnumerable<int>> GetNomenclatureGroupsAsync(ObjectType objectType)
        {
            var result = await _context.ObjectTypeNomenclatureGroups
                .Where(x => x.ObjectType == objectType)
                .Select(x => x.NomenclatureGroupId)
                .ToArrayAsync();

            return result;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> SaveNomenclatureGroupsAsync(ObjectType objectType, IEnumerable<int> groupIdList)
        {
            if (groupIdList == null)
                groupIdList = new int[0];

            var errors = new List<string>();
            try
            {
                var existingGroups = await _context.ObjectTypeNomenclatureGroups
                    .Where(x => x.ObjectType == objectType)
                    .ToArrayAsync();

                var groupIdsToDelete = existingGroups.Select(x => x.NomenclatureGroupId)
                    .Except(groupIdList)
                    .ToArray();
                if (groupIdsToDelete.Any())
                {
                    var groupsToDelete = existingGroups.Where(x => groupIdsToDelete.Contains(x.NomenclatureGroupId));
                    _context.ObjectTypeNomenclatureGroups.RemoveRange(groupsToDelete);
                }

                var groupIdsToAdd = groupIdList
                    .Except(existingGroups.Select(x => x.NomenclatureGroupId))
                    .ToArray();
                if (groupIdsToAdd.Any())
                {
                    foreach (var id in groupIdsToAdd)
                    {
                        _context.ObjectTypeNomenclatureGroups.Add(new ObjectTypeNomenclatureGroup 
                        { 
                            ObjectType = objectType,
                            NomenclatureGroupId = id,
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return (true, null);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Ошибка сохранения групп номенклатуры по типу объекта в системе.");
            }

            return (false, errors);
        }
    }
}
