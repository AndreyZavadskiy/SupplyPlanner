using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SP.Core.Master;
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
        Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync();
        Task<int> LinkInventoryToNomenclatureAsync(int[] inventoryIdList, int nomenclatureId);
        Task<int> BlockInventoryAsync(int[] inventoryIdList);
        Task<IEnumerable<InventoryBalanceListItem>> GetBalanceListAsync();
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
                    NomenclatureGroupName = x.NomenclatureGroup.Name,
                    UsefulLife = x.UsefulLife
                })
                .ToArrayAsync();

            return list;
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

        public async Task<IEnumerable<InventoryBalanceListItem>> GetBalanceListAsync()
        {
            try
            {
                var list = await _context.NomenclatureBalance
                    .Include(x => x.Nomenclature)
                    .Include(x => x.GasStation)
                    .Include(x => x.Nomenclature.MeasureUnit)
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
