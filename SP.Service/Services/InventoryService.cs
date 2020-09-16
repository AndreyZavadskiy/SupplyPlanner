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
        Task<IEnumerable<MergingInventory>> GetListForManualMerge(int? mergeType);
        Task<NomenclatureModel> GetNomenclatureModelAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveNomenclatureAsync(NomenclatureModel model);
        Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync();
        Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int? groupId, bool longterm);
        Task<int> LinkInventoryToNomenclatureAsync(int[] inventoryIdList, int nomenclatureId);
        Task<int> BlockInventoryAsync(int[] inventoryIdList);
        Task<IEnumerable<BalanceListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom);
        Task<IEnumerable<DemandListItem>> GetDemandListAsync(int? region, int? terr, int? station, int? group, int? nom);
        Task<IEnumerable<OrderModel>> GetOrderListAsync();
        Task<IEnumerable<OrderDetailModel>> GetOrderDetailAsync(int id);
        Task<int> SetRequirementAsync(decimal? fixedAmount, string formula, int[] idList);
        Task<(int OrderNumber, int RecordCount)> SaveOrderAsync(IEnumerable<OrderQuantity> data, int personId);
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

        /// <summary>
        /// Очистить данные по ТМЦ по конкретному пользователю в stage-таблице
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<bool> PurgeStageInventoryAsync(int personId)
        {
            await _context.StageInventories
                .Where(x => x.PersonId == personId)
                .BatchDeleteAsync();
            return true;
        }

        /// <summary>
        /// Сохранить данные по ТМЦ в stage-таблице
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serviceKey"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список ТМЦ для ручного объединения в Номенклатуру
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MergingInventory>> GetListForManualMerge(int? mergeType)
        {
            var query = _context.Inventories.AsNoTracking();
            switch (mergeType)
            {
                case 1:
                    query = query.Where(x => !x.NomenclatureId.HasValue && !x.IsBlocked);
                    break;
                case 2:
                    query = query.Where(x => x.NomenclatureId.HasValue && !x.IsBlocked);
                    break;
                case 3:
                    query = query.Where(x => x.IsBlocked);
                    break;
            }

            var mergingList = await query
                .Include(x => x.MeasureUnit)
                .Include(x => x.GasStation)
                .Include(x => x.Nomenclature)
                .Select(x => new MergingInventory
                {
                    Id = x.Id,
                    InventoryCode = x.Code,
                    InventoryName = x.Name,
                    MeasureUnitId = x.MeasureUnitId,
                    MeasureUnitName = x.MeasureUnit.Name,
                    GasStationId = x.GasStationId,
                    StationNumber = x.GasStation.StationNumber,
                    NomenclatureId = x.NomenclatureId,
                    NomenclatureCode = x.Nomenclature.Code,
                    NomenclatureName = x.Nomenclature.Name,
                    Active = x.IsBlocked ? "0" : "1"
                })
                .ToArrayAsync();

            return mergingList;
        }

        /// <summary>
        /// Получить позицию Номенклатуры для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список Номенклатуры для просмотра
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Получить позицию Номенклатуры
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int? groupId, bool longterm)
        {
            var query = _context.Nomenclatures.AsNoTracking();
            if (groupId != null)
            {
                query = query.Where(x => x.NomenclatureGroupId == groupId);
            }
            if (longterm)
            {
                query = query.Where(x => x.UsefulLife > 12);
            }
            var list = await query
                .Select(x => new DictionaryListItem
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToArrayAsync();

            return list;
        }

        /// <summary>
        /// Сохранить позицию Номенклатуры
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveNomenclatureAsync(NomenclatureModel model)
        {
            if (model.Id == 0)
            {
                return await CreateNomenclatureAsync(model);
            }

            return await UpdateNomenclatureAsync(model);

        }

        /// <summary>
        /// Создать позицию Номенклатуры
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> CreateNomenclatureAsync(NomenclatureModel model)
        {
            var nomenclature = new Nomenclature
            {
                Code = model.Code,
                Name = model.Name,
                PetronicsCode = model.PetronicsCode,
                PetronicsName = model.PetronicsName,
                MeasureUnitId = model.MeasureUnitId,
                NomenclatureGroupId = model.NomenclatureGroupId,
                UsefulLife = model.UsefulLife,
                IsActive = !model.Inactive
            };

            var errors = new List<string>();
            try
            {
                _context.Nomenclatures.Add(nomenclature);
                await _context.SaveChangesAsync();

                return (true, nomenclature.Id, null);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Ошибка создания номенклатуры в системе.");
            }

            return (false, null, errors);
        }

        /// <summary>
        /// Обновить позицию Номенклатуры
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateNomenclatureAsync(NomenclatureModel model)
        {
            var nomenclature = await _context.Nomenclatures.FindAsync(model.Id);
            if (nomenclature == null)
            {
                return (false, model.Id, new[] { "Номенклатура не найдена в базе данных." });
            }

            var errors = new List<string>();
            try
            {
                nomenclature.Code = model.Code;
                nomenclature.Name = model.Name;
                nomenclature.PetronicsCode = model.PetronicsCode;
                nomenclature.PetronicsName = model.PetronicsName;
                nomenclature.MeasureUnitId = model.MeasureUnitId;
                nomenclature.NomenclatureGroupId = model.NomenclatureGroupId;
                nomenclature.UsefulLife = model.UsefulLife;
                nomenclature.IsActive = !model.Inactive;

                _context.Nomenclatures.Update(nomenclature);
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

        /// <summary>
        /// Присоединить ТМЦ к Номенклатуре
        /// </summary>
        /// <param name="inventoryIdList"></param>
        /// <param name="nomenclatureId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Заблокировать ТМЦ от присоединения к Номенклатуре
        /// </summary>
        /// <param name="inventoryIdList"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить список остатков по Номенклатуре для просмотра
        /// </summary>
        /// <param name="region"></param>
        /// <param name="terr"></param>
        /// <param name="station"></param>
        /// <param name="group"></param>
        /// <param name="nom"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BalanceListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom)
        {
            try
            {
                var query = _context.CalcSheets.AsQueryable();
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
                    .Select(x => new BalanceListItem
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

        /// <summary>
        /// Получить список остатков и потребности по Номенклатуре для просмотра
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DemandListItem>> GetDemandListAsync(int? region, int? terr, int? station, int? group, int? nom)
        {
            try
            {
                var query = _context.CalcSheets.AsQueryable();
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

                var orderList = await query
                    .Select(x => new DemandListItem
                        {
                            Id = x.Id,
                            Code = x.Nomenclature.Code ?? x.Nomenclature.Id.ToString(),
                            Name = x.Nomenclature.Name,
                            GasStationName = x.GasStation.StationNumber,
                            Quantity = x.Quantity,
                            MeasureUnitName = x.Nomenclature.MeasureUnit.Name,
                            FixedAmount = x.FixedAmount,
                            Formula = x.Formula,
                            Plan = x.Plan,
                            OrderQuantity = x.Plan - x.Quantity
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

        /// <summary>
        /// Сохранить потребность по Номенклатуре
        /// </summary>
        /// <param name="fixedAmount"></param>
        /// <param name="formula"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public async Task<int> SetRequirementAsync(decimal? fixedAmount, string formula, int[] idList)
        {
            var existingRecords = await _context.CalcSheets
                .Where(b => idList.Contains(b.Id))
                .ToArrayAsync();

            foreach (var rec in existingRecords)
            {
                rec.FixedAmount = fixedAmount;
                rec.Formula = formula;
                _context.Entry(rec).Property(r => r.FixedAmount).IsModified = true;
                _context.Entry(rec).Property(r => r.Formula).IsModified = true;
            }

            var updated = await _context.SaveChangesAsync();

            var newIdList = idList.Except(
                existingRecords.Select(e => e.Id)
                )
                .ToArray();
            var nomCalculations = await _context.CalcSheets
                .Where(x => newIdList.Contains(x.Id))
                .ToArrayAsync();
            foreach (var item in nomCalculations)
            {
                var newRec = new CalcSheet
                {

                    NomenclatureId = item.NomenclatureId,
                    GasStationId = item.GasStationId,
                    FixedAmount = fixedAmount,
                    Formula = formula
                };

                await _context.CalcSheets.AddAsync(newRec);
            }

            var inserted = await _context.SaveChangesAsync();

            return updated + inserted;
        }

        /// <summary>
        /// Сохранить заказ по Номенклатуре
        /// </summary>
        /// <param name="data"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<(int OrderNumber, int RecordCount)> SaveOrderAsync(IEnumerable<OrderQuantity> data, int personId)
        {
            if (data == null || !data.Any())
            {
                return (0, 0);
            }

            try
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    PersonId = personId
                };

                var orderDetails = _context.CalcSheets.AsEnumerable()
                    .Join(data,
                        b => b.Id,
                        d => d.Id,
                        (b, d) => new OrderDetail
                        {
                            Order = order,
                            NomenclatureId = b.NomenclatureId,
                            GasStationId = b.GasStationId,
                            Quantity = d.Quantity
                        })
                    .ToArray();

                await _context.Orders.AddAsync(order);
                await _context.OrderDetails.AddRangeAsync(orderDetails);
                int saved = await _context.SaveChangesAsync();

                return (order.Id, saved);
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return (0, 0);
        }

        /// <summary>
        /// Получить список заказов для просмотра
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<OrderModel>> GetOrderListAsync()
        {
            var list = await _context.Orders
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderModel
                {
                    Id = x.Id,
                    OrderDate = x.OrderDate,
                    PersonName = $"{x.Person.LastName} {x.Person.FirstName}"
                })
                .ToArrayAsync();

            return list;
        }

        /// <summary>
        /// Получить позиции заказа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDetailModel>> GetOrderDetailAsync(int id)
        {
            var list = await _context.OrderDetails
                .Where(x => x.OrderId == id)
                .Select(x => new OrderDetailModel
                {
                    Id = x.Id,
                    Code = x.Nomenclature.Code ?? x.Nomenclature.Id.ToString(),
                    Name = x.Nomenclature.Name,
                    MeasureUnitName = x.Nomenclature.MeasureUnit.Name,
                    GasStationName = x.GasStation.StationNumber,
                    UsefulLife = x.Nomenclature.UsefulLife,
                    Quantity = x.Quantity
                })
                .ToArrayAsync();
            return list;
        }

        /// <summary>
        /// Перенести данные по ТМЦ из stage-таблицы в основную
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
