using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SP.Core.Model;
using SP.Data;
using SP.Service.Models;

namespace SP.Service.Services
{
    public interface IGasStationService
    {
        Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int? territoryId);
        Task<IEnumerable<GasStationIdentification>> GetGasStationIdentificationListAsync();
        Task<GasStationModel> GetGasStationAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveGasStationAsync(GasStationModel model);
    }

    public class GasStationService : IGasStationService
    {
        private readonly ApplicationDbContext _context;

        public GasStationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int? territoryId)
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
                .Select(x => new GasStationListItem
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

        public async Task<IEnumerable<GasStationIdentification>> GetGasStationIdentificationListAsync()
        {
            var list = await _context.GasStations.AsNoTracking()
                .Select(x => new GasStationIdentification
                {
                    Id = x.Id,
                    CodeSAP = x.CodeSAP
                })
                .ToArrayAsync();

            return list;
        }

        public async Task<GasStationModel> GetGasStationAsync(int id)
        {
            var station = await _context.GasStations.FindAsync(id);

            if (station == null)
            {
                return null;
            }

            var territory = await _context.RegionStructure.FindAsync(station.TerritoryId);
            if (territory == null || territory.ParentId == null)
            {
                return null;
            }

            var model = new GasStationModel
            {
                Id = station.Id,
                Code = string.IsNullOrWhiteSpace(station.Code) ? station.Id.ToString() : station.Code,
                CodeKSSS = station.CodeKSSS,
                CodeSAP = station.CodeSAP,
                StationNumber = station.StationNumber,
                RegionId = territory.ParentId.Value,
                TerritoryId = station.TerritoryId,
                SettlementId = station.SettlementId,
                Address = station.Address,
                StationLocationId = station.StationLocationId,
                StationStatusId = station.StationStatusId,
                ServiceLevelId = station.ServiceLevelId,
                OperatorRoomFormatId = station.OperatorRoomFormatId,
                ManagementSystemId = station.ManagementSystemId,
                TradingHallOperatingModeId = station.TradingHallOperatingModeId,
                ClientRestroomId = station.ClientRestroomId,
                CashboxLocationId = station.CashboxLocationId,
                TradingHallSizeId = station.TradingHallSizeId,
                CashboxTotal = station.CashboxTotal,
                PersonnelPerDay = station.PersonnelPerDay,
                FuelDispenserTotal = station.FuelDispenserTotal,
                ClientRestroomTotal = station.ClientRestroomTotal,
                TradingHallArea = station.TradingHallArea,
                ChequePerDay = station.ChequePerDay,
                RevenueAvg = station.RevenueAvg,
                HasSibilla = station.HasSibilla,
                HasBakery = station.HasBakery,
                HasCakes = station.HasCakes,
                HasFrenchFry = station.HasFrenchFry,
                HasMarmite = station.HasMarmite,
                HasKitchen = station.HasKitchen,
                CoffeeMachineTotal = station.CoffeeMachineTotal,
                DishWashingMachineTotal = station.DishWashingMachineTotal,
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveGasStationAsync(GasStationModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                return await InsertUserAsync(model);
            }

            return await UpdateUserAsync(dbStation, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertUserAsync(GasStationModel model)
        {
            var errors = new List<string>();
            try
            {
                var gasStation = new GasStation
                {
                    Code = model.Code,
                    CodeKSSS = model.CodeKSSS,
                    CodeSAP = model.CodeSAP,
                    StationNumber = model.StationNumber,
                    TerritoryId = model.TerritoryId,
                    SettlementId = model.SettlementId,
                    Address = model.Address,
                    StationLocationId = model.StationLocationId,
                    StationStatusId = model.StationStatusId,
                    ServiceLevelId = model.ServiceLevelId,
                    OperatorRoomFormatId = model.OperatorRoomFormatId,
                    ManagementSystemId = model.ManagementSystemId,
                    TradingHallOperatingModeId = model.TradingHallOperatingModeId,
                    ClientRestroomId = model.ClientRestroomId,
                    CashboxLocationId = model.CashboxLocationId,
                    TradingHallSizeId = model.TradingHallSizeId,
                    CashboxTotal = model.CashboxTotal,
                    PersonnelPerDay = model.PersonnelPerDay,
                    FuelDispenserTotal = model.FuelDispenserTotal,
                    ClientRestroomTotal = model.ClientRestroomTotal,
                    TradingHallArea = model.TradingHallArea,
                    ChequePerDay = model.ChequePerDay,
                    RevenueAvg = model.RevenueAvg,
                    HasJointRestroomEntrance = model.HasJointRestroomEntrance,
                    HasSibilla = model.HasSibilla,
                    HasBakery = model.HasBakery,
                    HasCakes = model.HasCakes,
                    HasFrenchFry = model.HasFrenchFry,
                    HasMarmite = model.HasMarmite,
                    HasKitchen = model.HasKitchen,
                    CoffeeMachineTotal = model.CoffeeMachineTotal,
                    DishWashingMachineTotal = model.DishWashingMachineTotal
                };

                _context.GasStations.Add(gasStation);
                await _context.SaveChangesAsync();
                //_logger.LogInformation("User created a new account with password.");

                return (true, gasStation.Id, null);
            }
            catch (DbUpdateException ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Невозможно сохранить изменения в базе данных. Если ошибка повторится, обратитесь в тех.поддержку.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                errors.Add("Ошибка создания персоны в системе.");
            }

            return (false, null, errors);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateUserAsync(GasStation station, GasStationModel model)
        {
            var errors = new List<string>();

            try
            {
                station.Code = model.Code;
                station.CodeKSSS = model.CodeKSSS;
                station.CodeSAP = model.CodeSAP;
                station.StationNumber = model.StationNumber;
                station.TerritoryId = model.TerritoryId;
                station.SettlementId = model.SettlementId;
                station.Address = model.Address;
                station.StationLocationId = model.StationLocationId;
                station.StationStatusId = model.StationStatusId;
                station.ServiceLevelId = model.ServiceLevelId;
                station.OperatorRoomFormatId = model.OperatorRoomFormatId;
                station.ManagementSystemId = model.ManagementSystemId;
                station.TradingHallOperatingModeId = model.TradingHallOperatingModeId;
                station.ClientRestroomId = model.ClientRestroomId;
                station.CashboxLocationId = model.CashboxLocationId;
                station.TradingHallSizeId = model.TradingHallSizeId;
                station.CashboxTotal = model.CashboxTotal;
                station.PersonnelPerDay = model.PersonnelPerDay;
                station.FuelDispenserTotal = model.FuelDispenserTotal;
                station.ClientRestroomTotal = model.ClientRestroomTotal;
                station.TradingHallArea = model.TradingHallArea;
                station.ChequePerDay = model.ChequePerDay;
                station.RevenueAvg = model.RevenueAvg;
                station.HasJointRestroomEntrance = model.HasJointRestroomEntrance;
                station.HasSibilla = model.HasSibilla;
                station.HasBakery = model.HasBakery;
                station.HasCakes = model.HasCakes;
                station.HasFrenchFry = model.HasFrenchFry;
                station.HasMarmite = model.HasMarmite;
                station.HasKitchen = model.HasKitchen;
                station.CoffeeMachineTotal = model.CoffeeMachineTotal;
                station.DishWashingMachineTotal = model.DishWashingMachineTotal;
                _context.GasStations.Update(station);
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
                errors.Add("Ошибка сохранения персоны в системе.");
            }

            return (false, model.Id, errors);
        }
    }
}
