using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
        Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync(int? groupId);
        Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int[] groups, bool longterm);
        Task<IEnumerable<NomenclatureInventory>> GetNomenclatureInventoryListAsync(int id);
        Task<int> LinkInventoryWithNomenclatureAsync(int[] inventoryIdList, int nomenclatureId, int personId);
        Task<int> BlockInventoryAsync(int[] inventoryIdList, int personId);
        Task<IEnumerable<BalanceListItem>> GetBalanceListAsync(int[] regions, int[] terrs, int? station, int? group, int? nom, bool zero);
        Task<IEnumerable<DemandListItem>> GetDemandListAsync(int[] regions, int[] terrs, int? station, int? group, int? nom);
        Task<IEnumerable<OrderModel>> GetOrderListAsync();
        Task<IEnumerable<OrderDetailModel>> GetOrderDetailAsync(int id);
        Task<int> SetRequirementAsync(decimal? fixedAmount, string formula, int[] idList);
        Task<(int OrderNumber, int RecordCount)> SaveOrderAsync(int orderType, bool withBalance, IEnumerable<OrderQuantity> data, int personId);
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
            string sqlStatement = "DELETE FROM stage.Inventory WHERE PersonId = @PersonId;";
            var p1 = new SqlParameter("@PersonId", personId);
            int deleted = await _context.Database.ExecuteSqlRawAsync(sqlStatement, p1);
            return true;
        }

        /// <summary>
        /// Сохранить данные по ТМЦ
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
            string stepMessage = "[1/2] Сохранение остатков ТМЦ в базу данных";
            UpdateProgress(serviceKey, stepMessage, 0);

            // сохраняем в stage
            while (true)
            {
                var portion = data
                    .Skip(currentRow)
                    .Take(250)
                    .ToArray();
                if (portion.Length == 0)
                {
                    break;
                }

                await _context.StageInventories.AddRangeAsync(portion);
                await _context.SaveChangesAsync();
                currentRow += 250;
                UpdateProgress(serviceKey, stepMessage, 100.0m * currentRow / totalRows);
            }

            // переносим в основную таблицу
            UpdateProgress(serviceKey, "[2/2] Обновление остатков ТМЦ в базе данных", 100.0m * currentRow / totalRows);
            var p1 = new SqlParameter("@PersonId", personId);
            var p2 = new SqlParameter("@Rows", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            await _context.Database.ExecuteSqlRawAsync("dbo.MergeStageToInventory @PersonId, @Rows OUT", p1, p2);
            UpdateProgress(serviceKey, "[2/2] Обновление остатков ТМЦ в базе данных", 100.0m);
            Debug.WriteLine(p2.Value.ToString());

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
        public async Task<IEnumerable<NomenclatureListItem>> GetNomenclatureListAsync(int? groupId)
        {
            var query = _context.Nomenclatures
                .Include(x => x.MeasureUnit)
                .Include(x => x.NomenclatureGroup)
                .AsNoTracking();
            if (groupId != null)
            {
                query = query.Where(x => x.NomenclatureGroupId == groupId);
            }

            var list = await query
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
        public async Task<IEnumerable<DictionaryListItem>> GetNomenclatureListItemsAsync(int[] groups, bool longterm)
        {
            var query = _context.Nomenclatures.AsNoTracking();
            if (groups != null)
            {
                query = query.Where(x => groups.Contains(x.NomenclatureGroupId));
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

        public async Task<IEnumerable<NomenclatureInventory>> GetNomenclatureInventoryListAsync(int id)
        {
            var list = await _context.Inventories
                .Include(x => x.Nomenclature)
                .Include(x => x.GasStation)
                .Where(x => x.NomenclatureId == id)
                .Select(x => new NomenclatureInventory
                {
                    Code = x.Code,
                    Name = x.Name,
                    StationNumber = x.GasStation.StationNumber
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
        public async Task<int> LinkInventoryWithNomenclatureAsync(int[] inventoryIdList, int nomenclatureId, int personId)
        {
            Stopwatch sw = new Stopwatch();
            int currentRow = 0;
            int updated = 0;
            while (true)
            {
                var portion = inventoryIdList
                    .Skip(currentRow)
                    .Take(250)
                    .ToArray();
                if (portion.Length == 0)
                {
                    break;
                }

                var p1 = new SqlParameter("@PersonId", personId);
                var p2 = new SqlParameter("@IdList" , string.Join(',', portion));
                var p3 = new SqlParameter("@NomenclatureId", nomenclatureId);
                var pRows = new SqlParameter("@Rows", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "dbo.LinkInventoryListWithNomenclature @PersonId, @IdList, @NomenclatureId, @Rows OUT", 
                    p1, p2, p3, pRows);
                updated += (pRows.Value == null || pRows.Value == DBNull.Value) ? 0 : (int)pRows.Value;
                currentRow += 250;
            }

            sw.Stop();
            Debug.WriteLine($"Обработано записей {inventoryIdList.Length}, присоединено {updated} за {sw.ElapsedMilliseconds} мс");

            return updated;
        }

        /// <summary>
        /// Заблокировать ТМЦ от присоединения к Номенклатуре
        /// </summary>
        /// <param name="inventoryIdList"></param>
        /// <returns></returns>
        public async Task<int> BlockInventoryAsync(int[] inventoryIdList, int personId)
        {
            Stopwatch sw = new Stopwatch();
            int currentRow = 0;
            int updated = 0;
            while (true)
            {
                var portion = inventoryIdList
                    .Skip(currentRow)
                    .Take(250)
                    .ToArray();
                if (portion.Length == 0)
                {
                    break;
                }

                var p1 = new SqlParameter("@PersonId", personId);
                var p2 = new SqlParameter("@IdList", string.Join(',', portion));
                var pRows = new SqlParameter("@Rows", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "dbo.BlockInventoryList @PersonId, @IdList, @Rows OUT",
                    p1, p2, pRows);
                updated += (pRows.Value == null || pRows.Value == DBNull.Value) ? 0 : (int)pRows.Value;
                currentRow += 250;
            }

            sw.Stop();
            Debug.WriteLine($"Обработано записей {inventoryIdList.Length}, присоединено {updated} за {sw.ElapsedMilliseconds} мс");

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
        public async Task<IEnumerable<BalanceListItem>> GetBalanceListAsync(int[] regions, int[] terrs, int? station, int? group, int? nom, bool hideZero)
        {
            try
            {
                var query = _context.CalcSheets.AsQueryable();
                if (station.HasValue)
                {
                    query = query.Where(x => x.GasStationId == station);
                }
                else if (terrs?.Length > 0)
                {
                    query = query.Where(x => terrs.Contains(x.GasStation.TerritoryId));
                }
                else if (regions?.Length > 0)
                {
                    query = query.Where(x => x.GasStation.Territory.ParentId != null && regions.Contains(x.GasStation.Territory.ParentId.Value));
                }

                if (nom.HasValue)
                {
                    query = query.Where(x => x.NomenclatureId == nom);
                }
                else if (group.HasValue)
                {
                    query = query.Where(x => x.Nomenclature.NomenclatureGroupId == group);
                }

                if (hideZero)
                {
                    query = query.Where(x => x.Quantity != 0);
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
            catch (Exception)
            {
                Debugger.Break();
            }

            return null;
        }

        /// <summary>
        /// Получить список остатков и потребности по Номенклатуре для просмотра
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DemandListItem>> GetDemandListAsync(int[] regions, int[] terrs, int? station, int? group, int? nom)
        {
            try
            {
                var query = _context.CalcSheets.AsQueryable();
                if (station.HasValue)
                {
                    query = query.Where(x => x.GasStationId == station);
                }
                else if (terrs?.Length > 0)
                {
                    query = query.Where(x => terrs.Contains(x.GasStation.TerritoryId));
                }
                else if (regions?.Length > 0)
                {
                    query = query.Where(x => x.GasStation.Territory.ParentId != null && regions.Contains(x.GasStation.Territory.ParentId.Value));
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
            catch (Exception)
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
        public async Task<(int OrderNumber, int RecordCount)> SaveOrderAsync(int orderType, bool withBalance, IEnumerable<OrderQuantity> data, int personId)
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

                OrderDetail[] orderDetails;
                if (orderType == 1)
                {
                    orderDetails = _context.CalcSheets
                        .Include(b => b.Nomenclature)
                        .Where(b => b.Nomenclature.UsefulLife <= 12)
                        .AsEnumerable()
                        .Join(data.Where(d => d.Plan != 0),
                            b => b.Id,
                            d => d.Id,
                            (b, d) => new OrderDetail
                            {
                                Order = order,
                                NomenclatureId = b.NomenclatureId,
                                GasStationId = b.GasStationId,
                                Quantity = d.Plan
                            })
                        .ToArray();
                }
                else
                {
                    orderDetails = _context.CalcSheets
                        .Include(b => b.Nomenclature)
                        .AsEnumerable()
                        .Join(data,
                            b => b.Id,
                            d => d.Id,
                            (b, d) => new OrderDetail
                            {
                                Order = order,
                                NomenclatureId = b.NomenclatureId,
                                GasStationId = b.GasStationId,
                                Quantity = withBalance ? d.Quantity : d.Plan
                            })
                        .Where(z => z.Quantity != 0)
                        .ToArray();
                }

                await _context.Orders.AddAsync(order);
                await _context.OrderDetails.AddRangeAsync(orderDetails);
                int saved = await _context.SaveChangesAsync();

                return (order.Id, saved);
            }
            catch (Exception)
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
