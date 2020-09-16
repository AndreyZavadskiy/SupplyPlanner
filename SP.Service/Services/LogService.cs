using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        Task<IEnumerable<ChangeListItem>> GetChangeListAsync(int? user, DateTime? start, DateTime? end);

        Task<IEnumerable<CalcSheetReportListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group,
            int? nom, DateTime? start, DateTime? end);
        Task<IEnumerable<CalcSheetReportListItem>> GetExceedPlanListAsync(int? region, int? terr, int? station, int? group,
            int? nom, DateTime? start, DateTime? end);
        Task<IEnumerable<OrderDetailReportListItem>> GetOrderListAsync(int? region, int? terr, int? station, int? group,
            int? nom, DateTime? start, DateTime? end);
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
        public async Task<IEnumerable<ChangeListItem>> GetChangeListAsync(int? user, DateTime? start, DateTime? end)
        {
            var query = _context.ChangeLogs.AsNoTracking();
            if (user != null)
            {
                query = query.Where(x => x.PersonId == user);
            }
            if (start != null)
            {
                query = query.Where(x => start <= x.ChangeDate);
            }
            if (end != null)
            {
                end = end.Value.Date.AddDays(1);
                query = query.Where(x => x.ChangeDate < end);
            }

            var result = await query
                .Include(x => x.Person)
                .Select(x => new
                {
                    x.Id,
                    x.Person.LastName,
                    x.Person.FirstName,
                    x.Person.MiddleName,
                    x.ChangeDate,
                    x.EntityName,
                    x.RecordId,
                    x.OldValue,
                    x.NewValue
                })
                .OrderBy(z => z.ChangeDate)
                .Take(5000)
                .ToArrayAsync();

            var list = result.Select(x => new ChangeListItem
            {
                Id = x.Id,
                UserName = Person.ConcatenateFio(x.LastName, x.FirstName, x.MiddleName),
                ChangeDate = x.ChangeDate,
                EntityName = x.EntityName,
                RecordId = x.RecordId,
                OldValue = x.OldValue,
                NewValue = x.NewValue
            });

            return list;
        }

        public async Task<IEnumerable<CalcSheetReportListItem>> GetBalanceListAsync(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            var query = _context.CalcSheetHistories.AsNoTracking();
            if (station != null)
            {
                query = query.Where(x => x.GasStationId == station);
            }
            else if (terr != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Where(x => x.GasStation.TerritoryId == terr);
            }
            else if (region != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Include(x => x.GasStation.Territory)
                    .Where(x => x.GasStation.Territory.ParentId == region);
            }
            if (nom != null)
            {
                query = query.Where(x => x.NomenclatureId == nom);
            }
            else if (group != null)
            {
                query = query
                    .Include(x => x.Nomenclature)
                    .Where(x => x.Nomenclature.NomenclatureGroupId == group);
            }
            if (start != null)
            {
                query = query.Where(x => start <= x.EffectiveDate);
            }
            if (end != null)
            {
                end = end.Value.Date.AddDays(1);
                query = query.Where(x => x.EffectiveDate < end);
            }

            var list = await query
                .Include(x => x.Nomenclature)
                .Include(x => x.Nomenclature.NomenclatureGroup)
                .Include(x => x.GasStation)
                .Where(x => x.Nomenclature.UsefulLife > 12)
                .Select(x => new CalcSheetReportListItem
                {
                    NomenclatureGroupName = x.Nomenclature.NomenclatureGroup.Name,
                    NomenclatureName = x.Nomenclature.Name,
                    StationNumber = x.GasStation.StationNumber,
                    Date = x.EffectiveDate,
                    Quantity = x.Quantity
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<IEnumerable<CalcSheetReportListItem>> GetExceedPlanListAsync(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            var query = _context.CalcSheetHistories.AsNoTracking();
            if (station != null)
            {
                query = query.Where(x => x.GasStationId == station);
            }
            else if (terr != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Where(x => x.GasStation.TerritoryId == terr);
            }
            else if (region != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Include(x => x.GasStation.Territory)
                    .Where(x => x.GasStation.Territory.ParentId == region);
            }
            if (nom != null)
            {
                query = query.Where(x => x.NomenclatureId == nom);
            }
            else if (group != null)
            {
                query = query
                    .Include(x => x.Nomenclature)
                    .Where(x => x.Nomenclature.NomenclatureGroupId == group);
            }
            if (start != null)
            {
                query = query.Where(x => start <= x.EffectiveDate);
            }
            if (end != null)
            {
                end = end.Value.Date.AddDays(1);
                query = query.Where(x => x.EffectiveDate < end);
            }

            var list = await query
                .Include(x => x.Nomenclature)
                .Include(x => x.Nomenclature.NomenclatureGroup)
                .Include(x => x.GasStation)
                .Where(x => x.Quantity > x.Plan)
                .Select(x => new CalcSheetReportListItem
                {
                    NomenclatureGroupName = x.Nomenclature.NomenclatureGroup.Name,
                    NomenclatureName = x.Nomenclature.Name,
                    StationNumber = x.GasStation.StationNumber,
                    Date = x.EffectiveDate,
                    Plan = x.Plan,
                    Quantity = x.Quantity
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<IEnumerable<OrderDetailReportListItem>> GetOrderListAsync(int? region, int? terr, int? station, int? group, int? nom, DateTime? start, DateTime? end)
        {
            var query = _context.OrderDetails.AsNoTracking();
            if (station != null)
            {
                query = query.Where(x => x.GasStationId == station);
            }
            else if (terr != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Where(x => x.GasStation.TerritoryId == terr);
            }
            else if (region != null)
            {
                query = query
                    .Include(x => x.GasStation)
                    .Include(x => x.GasStation.Territory)
                    .Where(x => x.GasStation.Territory.ParentId == region);
            }
            if (nom != null)
            {
                query = query.Where(x => x.NomenclatureId == nom);
            }
            else if (group != null)
            {
                query = query
                    .Include(x => x.Nomenclature)
                    .Where(x => x.Nomenclature.NomenclatureGroupId == group);
            }
            if (start != null)
            {
                query = query
                    .Include(x => x.Order)
                    .Where(x => start <= x.Order.OrderDate);
            }
            if (end != null)
            {
                end = end.Value.Date.AddDays(1);
                query = query
                    .Include(x => x.Order)
                    .Where(x => x.Order.OrderDate < end);
            }

            var list = await query
                .Include(x => x.Nomenclature)
                .Include(x => x.Nomenclature.NomenclatureGroup)
                .Include(x => x.GasStation)
                .Include(x => x.Order)
                .Select(x => new OrderDetailReportListItem
                {
                    NomenclatureGroupName = x.Nomenclature.NomenclatureGroup.Name,
                    NomenclatureName = x.Nomenclature.Name,
                    StationNumber = x.GasStation.StationNumber,
                    Date = x.Order.OrderDate,
                    Quantity = x.Quantity,
                    OrderNumber = x.Order.Id
                })
                .ToArrayAsync();

            return list;
        }
    }
}
