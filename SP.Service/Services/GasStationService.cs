using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IGasStationService
    {
        Task<IEnumerable<GasStationModel>> GetGasStationList(int? territoryId);
    }

    public class GasStationService : IGasStationService
    {
        private readonly ApplicationDbContext _context;

        public GasStationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GasStationModel>> GetGasStationList(int? territoryId)
        {
            var query = _context.GasStations.AsNoTracking();
            if (territoryId != null)
            {
                query = query.Where(x => x.TerritoryId == territoryId.Value);
            }

            var stations = await query
                .Join(_context.RegionStructure.AsNoTracking(),
                    s => s.Territory.ParentId,
                    r => r.Id,
                    (s, r) => new
                    {
                        Station = s,
                        RegionId = r.Id,
                        RegionName = r.Name
                    })
                .Select(x => new GasStationModel
                {
                    Id = x.Station.Id,
                    RegionId = x.RegionId,
                    RegionName = x.RegionName,
                    TerritoryId = x.Station.TerritoryId,
                    TerritoryName = x.Station.Territory.Name,
                    CodeKSSS = x.Station.CodeKSSS,
                    CodeSAP = x.Station.CodeSAP,
                    StationNumber = x.Station.StationNumber,
                    SettlementName = x.Station.Settlement.Name,
                    Address = x.Station.Address,
                    StationLocationName = x.Station.StationLocation.Name,
                    StationStatusName = x.Station.StationStatus.Name,
                    ServiceLevelName = x.Station.ServiceLevel.Name,
                    OperatorRoomFormatName = x.Station.OperatorRoomFormat.Name,
                    ManagementSystemName = x.Station.ManagementSystem.Name,
                    TradingHallOperatingModeName = x.Station.TradingHallOperatingMode.Name,
                    ClientRestroomName = x.Station.ClientRestroom.Name,
                    CashboxLocationName = x.Station.CashboxLocation.Name,
                    TradingHallSizeName = x.Station.TradingHallSize.Name,
                    CashboxTotal = x.Station.CashboxTotal,
                    PersonnelPerDay = x.Station.PersonnelPerDay,
                    FuelDispenserTotal = x.Station.FuelDispenserTotal,
                    ClientRestroomTotal = x.Station.ClientRestroomTotal,
                    TradingHallArea = x.Station.TradingHallArea,
                    ChequePerDay = x.Station.ChequePerDay,
                    RevenueAvg = x.Station.RevenueAvg,
                    HasSibilla = x.Station.HasSibilla,
                    HasBakery = x.Station.HasBakery,
                    HasCakes = x.Station.HasCakes,
                    HasFrenchFry = x.Station.HasFrenchFry,
                    HasMarmite = x.Station.HasMarmite,
                    HasKitchen = x.Station.HasKitchen,
                    CoffeeMachineTotal = x.Station.CoffeeMachineTotal,
                    DishWashingMachineTotal = x.Station.DishWashingMachineTotal
                })
                .ToArrayAsync();

            return stations;
        }
    }
}
