using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Internal;
using SP.Core.Model;
using SP.Data;
using SP.Service.DTO;

namespace SP.Service.Services
{
    public interface IInventoryService
    {
        Task<bool> PurgeStageInventoryAsync();
        Task<(bool Success, IEnumerable<string> Errors)> SaveStageInventoryAsync(IEnumerable<ParsedInventory> data);
    }

    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PurgeStageInventoryAsync()
        {
            await _context.StageInventories.Where(x => true).BatchDeleteAsync();
            return true;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> SaveStageInventoryAsync(IEnumerable<ParsedInventory> data)
        {
            if (data == null)
            {
                return (true, null);
            }

            // TODO сделать сравнение с существующими записями

            //var existingStageInventories = _context.StageInventories
            //    .Join(data.Where(d => d.GasStationId.HasValue),
            //        t => new
            //        {
            //            GasStationId = t.GasStationId, 
            //            InventoryCode = t.Code
            //        },
            //        d => new
            //        {
            //            GasStationId = d.GasStationId.Value, 
            //            d.InventoryCode
            //        },
            //        (t, d) => new
            //        {
            //            Id = t.Id,
            //            OldQuantity = t.Quantity,
            //            NewQuantity = d.Quantity
            //        }
            //    )
            //    .ToArray();

            var rowsToInsert = data
                .Select(x => new StageInventory
                {
                    Code = x.InventoryCode,
                    Name = x.InventoryName,
                    GasStationId = x.GasStationId.Value,
                    Quantity = x.Quantity,
                    LastUpdate = DateTime.Now
                })
                .ToArray();

            await _context.BulkInsertAsync<StageInventory>(rowsToInsert);

            return (true, null);
        }
    }
}
