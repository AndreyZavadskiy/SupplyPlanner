using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SP.Core.Model;
using SP.Data;
using SP.Service.Background;
using SP.Service.DTO;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IInventoryService
    {
        Task<bool> PurgeStageInventoryAsync(int personId);
        Task<(bool Success, IEnumerable<string> Errors)> SaveStageInventoryAsync(StageInventory[] data, Guid? serviceKey, int personId);
        Task<IEnumerable<MergingInventory>> GetListForManualMerge();
        Task<NomenclatureModel> GetNomenclatureModelAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveNomenclatureAsync(NomenclatureModel model);
        Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync();
        Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int groupId);
        Task<int> LinkInventoryToNomenclatureAsync(int[] inventoryIdList, int nomenclatureId);
        Task<int> BlockInventoryAsync(int[] inventoryIdList);
        Task<IEnumerable<InventoryBalanceListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom);
        Task<IEnumerable<InventoryOrderListItem>> GetOrderListAsync();
        Task<int> SetRequirementAsync(decimal? fixedAmount, string formula, int[] idList);
    }

    public class InventoryService : IInventoryService
    {
        private readonly IBackgroundCoordinator _coordinator;
        private readonly ApplicationDbContext _context;

        public InventoryService(IBackgroundCoordinator coordinator, ApplicationDbContext context)
        {
            _coordinator = coordinator;
            _context = context;
        }

        public async Task<bool> PurgeStageInventoryAsync(int personId)
        {
            await _context.StageInventories
                .Where(x => x.PersonId == personId)
                .BatchDeleteAsync();
            return true;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> SaveStageInventoryAsync(StageInventory[] data, Guid? serviceKey, int personId)
        {
            if (data == null)
            {
                return (true, null);
            }

            int currentRow = 0,
                totalRows = data.Length;
            string stepMessage = "Сохранение остатков ТМЦ в базу данных";
            UpdateProgress(serviceKey, stepMessage, 0);

            // сохраняем в stage
            while (true)
            {
                var portion = data
                    .Skip(currentRow)
                    .Take(100)
                    .ToArray();
                if (portion.Length == 0)
                {
                    break;
                }

                await _context.StageInventories.AddRangeAsync(portion);
                await _context.SaveChangesAsync();
                currentRow += 100;
                UpdateProgress(serviceKey, stepMessage, 50.0m * currentRow / totalRows);
            }

            // переносим в основную таблицу
            currentRow = 0;
            while (true)
            {
                var portion = _context.StageInventories
                    .Where(x => x.PersonId == personId)
                    .Skip(currentRow)
                    .Take(100)
                    .ToArray();
                if (portion.Length == 0)
                {
                    break;
                }

                await UpdateInventory(portion);
                currentRow += 100;
                UpdateProgress(serviceKey, stepMessage, 50.0m + 50.0m * currentRow / totalRows);
            }


            return (true, null);
        }

        public async Task<IEnumerable<MergingInventory>> GetListForManualMerge()
        {
            var mergingList = await _context.Inventories
                .Include(x => x.MeasureUnit)
                .Include(x => x.GasStation)
                .Where(x => !x.NomenclatureId.HasValue && !x.IsBlocked)
                .Select(x => new MergingInventory
                {
                    Id = x.Id,
                    InventoryCode = x.Code,
                    InventoryName = x.Name,
                    MeasureUnitId = x.MeasureUnitId,
                    MeasureUnitName = x.MeasureUnit.Name,
                    GasStationId = x.GasStationId,
                    StationNumber = x.GasStation.StationNumber,
                    NomenclatureId = null,
                    NomenclatureCode = null,
                    NomenclatureName = null,
                    Active = x.IsBlocked ? "0" : "1"
                })
                .ToArrayAsync();

            return mergingList;
        }

        public async Task<NomenclatureModel> GetNomenclatureModelAsync(int id)
        {
            var nomenclature = await _context.Nomenclatures.FindAsync(id);
            if (nomenclature == null)
            {
                return null;
            }

            var nomenclatureModel = new NomenclatureModel()
            {
                Id = nomenclature.Id,
                Code = nomenclature.Code ?? nomenclature.Id.ToString(),
                Name = nomenclature.Name,
                PetronicsCode = nomenclature.PetronicsCode,
                PetronicsName = nomenclature.PetronicsName,
                MeasureUnitId = nomenclature.MeasureUnitId,
                NomenclatureGroupId = nomenclature.NomenclatureGroupId,
                UsefulLife = nomenclature.UsefulLife,
                Inactive = !nomenclature.IsActive
            };

            return nomenclatureModel;
        }

        public async Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync()
        {
            var list = await _context.Nomenclatures
                .Include(x => x.MeasureUnit)
                .Include(x => x.NomenclatureGroup)
                .Select(x => new NomenclatureListItem
                {
                    Id = x.Id,
                    Code = x.Code ?? x.Id.ToString(),
                    Name = x.Name,
                    PetronicsCode = x.PetronicsCode,
                    PetronicsName = x.PetronicsName,
                    MeasureUnitName = x.MeasureUnit.Name,
                    NomenclatureGroupId = x.NomenclatureGroupId,
                    NomenclatureGroupName = x.NomenclatureGroup.Name,
                    UsefulLife = x.UsefulLife,
                    Active = x.IsActive ? "1" : "0"
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int groupId)
        {
            var list = await _context.Nomenclatures
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveNomenclatureAsync(NomenclatureModel model)
        {
            bool hasRegionWithSameName = await _context.RegionStructure.AnyAsync(x => x.Id != model.Id && x.Name == model.Name);
            if (hasRegionWithSameName)
            {
                return (false, null, new[] { "Регион с таким наименованием уже существует. Нельзя создавать дубли." });
            }

            if (model.Id == 0)
            {
                return await CreateNomenclatureAsync(model);
            }

            return await UpdateNomenclatureAsync(model);

        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> CreateNomenclatureAsync(NomenclatureModel model)
        {
            var nomenclature = new NomenclatureModel
            {
                Name = model.Name,
                ParentId = model.ParentId,
                IsActive = !model.Inactive
            };

            var errors = new List<string>();
            try
            {
                _context.Nomenclatures.Add(region);
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateNomenclatureAsync(NomenclatureModel model)
        {
            var region = await _context.Nomenclatures.FindAsync(model.Id);
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

        public async Task<int> LinkInventoryToNomenclatureAsync(int[] inventoryIdList, int nomenclatureId)
        {
            int updated = await _context.Inventories
                .Where(x => inventoryIdList.Contains(x.Id))
                .BatchUpdateAsync(x => new Inventory
                {
                    NomenclatureId = nomenclatureId,
                    IsBlocked = false
                });

            return updated;
        }

        public async Task<int> BlockInventoryAsync(int[] inventoryIdList)
        {
            int updated = await _context.Inventories
                .Where(x => inventoryIdList.Contains(x.Id))
                .BatchUpdateAsync(x => new Inventory
                {
                    NomenclatureId = null,
                    IsBlocked = true
                });

            return updated;
        }

        public async Task<IEnumerable<InventoryBalanceListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom)
        {
            try
            {
                var query = _context.NomenclatureBalance.AsQueryable();
                if (station.HasValue)
                {
                    query = query.Where(x => x.GasStationId == station);
                }
                else if (terr.HasValue)
                {
                    query = query.Where(x => x.GasStation.TerritoryId == terr);
                }
                else if (region.HasValue)
                {
                    query = query.Where(x => x.GasStation.Territory.ParentId == region);
                }

                if (nom.HasValue)
                {
                    query = query.Where(x => x.NomenclatureId == nom);
                }
                else if (group.HasValue)
                {
                    query = query.Where(x => x.Nomenclature.NomenclatureGroupId == group);
                }

                var list = await query
                    .Select(x => new InventoryBalanceListItem
                    {
                        Id = x.Id,
                        Code = x.Nomenclature.Code ?? x.Id.ToString(),
                        Name = x.Nomenclature.Name,
                        GasStationName = x.GasStation.StationNumber,
                        Quantity = x.Quantity,
                        MeasureUnitName = x.Nomenclature.MeasureUnit.Name,
                        LastUpdate = x.LastUpdate
                    })
                    .ToArrayAsync();

                return list;
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return null;
        }

        public async Task<IEnumerable<InventoryOrderListItem>> GetOrderListAsync()
        {
            try
            {
                var orderList = await _context.NomenclatureBalance
                    .GroupJoin(_context.Requirements,
                        b => new {b.NomenclatureId, b.GasStationId},
                        r => new {r.NomenclatureId, r.GasStationId},
                        (b, r) => new
                        {
                            NomBalance = b,
                            Requirement = r
                        })
                    .SelectMany(
                        x => x.Requirement.DefaultIfEmpty(),
                        (x, y) => new InventoryOrderListItem
                        {
                            Id = x.NomBalance.Id,
                            Code = x.NomBalance.Nomenclature.Code ?? x.NomBalance.Nomenclature.Id.ToString(),
                            Name = x.NomBalance.Nomenclature.Name,
                            GasStationName = x.NomBalance.GasStation.StationNumber,
                            Quantity = x.NomBalance.Quantity,
                            MeasureUnitName = x.NomBalance.Nomenclature.MeasureUnit.Name,
                            FixedAmount = y.FixedAmount,
                            Formula = y == null ? null : y.Formula,
                            Plan = y == null ? 0.0m : y.Plan,
                            OrderQuantity = y == null ? 0.0m : y.Plan - x.NomBalance.Quantity
                        })
                    .ToArrayAsync();

                return orderList;
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return null;
        }

        public async Task<int> SetRequirementAsync(decimal? fixedAmount, string formula, int[] idList)
        {
            var existingRecords = await _context.NomenclatureBalance
                .Where(b => idList.Contains(b.Id))
                .Join(_context.Requirements,
                    b => new { b.NomenclatureId, b.GasStationId },
                    r => new {r.NomenclatureId, r.GasStationId},
                    (r, b) => new
                    {
                        RequirementId = r.Id,
                        NomenclatureBalanceId = b.Id
                    }
                )
                .ToArrayAsync();

            foreach (var item in existingRecords)
            {
                var rec = new Requirement
                {
                    Id = item.RequirementId,
                    FixedAmount = fixedAmount,
                    Formula = formula
                };

                _context.Entry(rec).Property(r => r.FixedAmount).IsModified = true;
                _context.Entry(rec).Property(r => r.Formula).IsModified = true;
            }

            var updated = await _context.SaveChangesAsync();

            var newIdList = idList.Except(
                existingRecords.Select(e => e.NomenclatureBalanceId)
                )
                .ToArray();
            var nomBalanceList = await _context.NomenclatureBalance
                .Where(x => newIdList.Contains(x.Id))
                .ToArrayAsync();
            foreach (var nb in nomBalanceList)
            {
                var newRec = new Requirement
                {
                    NomenclatureId = nb.NomenclatureId,
                    GasStationId = nb.GasStationId,
                    FixedAmount = fixedAmount,
                    Formula = formula
                };

                await _context.Requirements.AddAsync(newRec);
            }

            var inserted = await _context.SaveChangesAsync();

            return updated + inserted;
        }

        private async Task UpdateInventory(IEnumerable<StageInventory> data)
        {
            // обновляем существующие записи
            var existingRecords = _context.Inventories.AsEnumerable()
                .Join(data,
                    i => new { i.GasStationId, i.Code },
                    d => new { d.GasStationId, d.Code },
                    (i, d) => new
                    {
                        Inventory = i,
                        StageInventory = d
                    })
                .ToArray();

            foreach (var rec in existingRecords)
            {
                var dbRecord = rec.Inventory;
                dbRecord.Quantity = rec.StageInventory.Quantity;
                dbRecord.LastUpdate = DateTime.Now;
                _context.Inventories.Attach(dbRecord);
                _context.Entry(dbRecord).Property(r => r.Quantity).IsModified = true;
                _context.Entry(dbRecord).Property(r => r.LastUpdate).IsModified = true;
            }

            await _context.SaveChangesAsync();

            // добавляем новые
            var newRecords = data
                .Where(d => !_context.Inventories.Any(i => i.GasStationId == d.GasStationId && i.Code == d.Code))
                .Select(d => new Inventory
                {
                    Code = d.Code,
                    Name = d.Name,
                    GasStationId = d.GasStationId,
                    MeasureUnitId = d.MeasureUnitId,
                    Quantity = d.Quantity,
                    LastUpdate = DateTime.Now
                })
                .ToArray();
            await _context.Inventories.AddRangeAsync(newRecords);
            await _context.SaveChangesAsync();
        }

        private void UpdateProgress(Guid? serviceKey, string stepMessage, decimal progress)
        {
            if (_coordinator == null || serviceKey == null)
            {
                return;
            }

            var savingProgress = new BackgroundServiceProgress
            {
                Key = serviceKey.Value,
                Status = BackgroundServiceStatus.Running,
                Step = stepMessage,
                Progress = progress,
                Log = null
            };
            _coordinator.AddOrUpdate(savingProgress);
        }
    }
}
