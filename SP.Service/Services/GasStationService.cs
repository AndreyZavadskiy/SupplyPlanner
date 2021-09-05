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
    public interface IGasStationService
    {
        Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int[] regions, int[] territories);
        Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int[] regions, int[] territories, int? personId);
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

        public async Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int[] regions, int[] territories)
        {
            return await GetGasStationListAsync(regions, territories, null);
        }

        public async Task<IEnumerable<GasStationListItem>> GetGasStationListAsync(int[] regions, int[] territories, int? personId)
        {
            var query = _context.GasStations.AsNoTracking()
                .Include(x => x.Territory)
                .Include(x => x.Territory.Parent)
                .Include(x => x.Settlement)
                .Include(x => x.StationLocation)
                .Include(x => x.StationStatus)
                .Include(x => x.Segment)
                .Include(x => x.ServiceLevel)
                .Include(x => x.OperatorRoomFormat)
                .Include(x => x.ManagementSystem)
                .Include(x => x.TradingHallOperatingMode)
                .Include(x => x.ClientRestroom)
                .Include(x => x.CashboxLocation)
                .Include(x => x.TradingHallSize)
                .Include(x => x.CashRegisterTape)
                .Where(x => x.ObjectType == Core.Enum.ObjectType.GasStation)
                .AsQueryable();
            if (regions != null)
            {
                var regionTerritories = _context.RegionStructure.AsEnumerable()
                    .Where(x => x.ParentId != null && regions.Contains(x.ParentId.Value))
                    .Select(x => x.Id)
                    .ToArray();
                query = query.Where(x => regionTerritories.Contains(x.TerritoryId));
            }
            if (territories != null)
            {
                query = query.Where(x => territories.Contains(x.TerritoryId));
            }
            else if (personId != null)
            {
                var managerTerritories = await _context.PersonTerritories
                    .Where(x => x.PersonId == personId)
                    .Select(x => x.RegionalStructureId)
                    .Distinct()
                    .ToArrayAsync();
                query = query.Where(x => managerTerritories.Contains(x.TerritoryId));
            }

            var stations = query.AsEnumerable()
                .Select(x => new GasStationListItem
                {
                    Id = x.Id,
                    RegionId = x.Territory.ParentId.Value,
                    RegionName = x.Territory.Parent?.Name,
                    TerritoryId = x.TerritoryId,
                    TerritoryName = x.Territory?.Name,
                    CodeKSSS = x.CodeKSSS.Value,
                    CodeSAP = x.CodeSAP,
                    StationNumber = x.StationNumber,
                    SettlementName = x.Settlement?.Name,
                    Address = x.Address,
                    StationLocationName = x.StationLocation?.Name,
                    StationStatusName = x.StationStatus?.Name,
                    SegmentName = x.Segment?.Name,
                    ServiceLevelName = x.ServiceLevel?.Name,
                    OperatorRoomFormatName = x.OperatorRoomFormat?.Name,
                    ManagementSystemName = x.ManagementSystem?.Name,
                    TradingHallOperatingModeName = x.TradingHallOperatingMode?.Name,
                    ClientRestroomName = x.ClientRestroom?.Name,
                    CashboxLocationName = x.CashboxLocation?.Name,
                    TradingHallSizeName = x.TradingHallSize?.Name,
                    CashboxTotal = x.CashboxTotal.Value,
                    ManagerArmTotal = x.ManagerArmTotal.Value,
                    PersonnelPerDay = x.PersonnelPerDay.Value,
                    FuelDispenserTotal = x.FuelDispenserTotal.Value,
                    FuelDispenserPostTotal = x.FuelDispenserPostTotal.Value,
                    FuelDispenserPostWithoutShedTotal = x.FuelDispenserPostWithoutShedTotal.Value,
                    ClientRestroomTotal = x.ClientRestroomTotal.Value,
                    ClientTambourTotal = x.ClientTambourTotal.Value,
                    ClientSinkTotal = x.ClientSinkTotal.Value,
                    TradingHallArea = x.TradingHallArea,
                    CashRegisterTapeName = x.CashRegisterTape?.Name,
                    ChequePerDay = x.ChequePerDay.Value,
                    RevenueAvg = x.RevenueAvg.Value,
                    HasSibilla = x.HasSibilla,
                    HasBakery = x.HasBakery,
                    HasCakes = x.HasCakes,
                    DeepFryTotal = x.DeepFryTotal.Value,
                    HasMarmite = x.HasMarmite,
                    HasKitchen = x.HasKitchen,
                    CoffeeMachineTotal = x.CoffeeMachineTotal.Value,
                    DishWashingMachineTotal = x.DishWashingMachineTotal.Value,
                    RepresentativenessFactor = x.RepresentativenessFactor.Value,
                    RepresentativenessFactor3Quarter = x.RepresentativenessFactor3Quarter.Value,
                    MerrychefTotal = x.MerrychefTotal.Value,
                    DayCleaningTotal = x.DayCleaningTotal.Value,
                    NightCleaningTotal = x.NightCleaningTotal.Value,
                    DayRefuelingTotal = x.DayRefuelingTotal.Value,
                    NightRefuelingTotal = x.NightRefuelingTotal.Value,
                    HasFuelCardProgram = x.HasFuelCardProgram
                })
                .ToArray();

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
                CodeKSSS = station.CodeKSSS.Value,
                CodeSAP = station.CodeSAP,
                StationNumber = station.StationNumber,
                RegionId = territory.ParentId.Value,
                TerritoryId = station.TerritoryId,
                SettlementId = station.SettlementId.Value,
                Address = station.Address,
                StationLocationId = station.StationLocationId.Value,
                StationStatusId = station.StationStatusId.Value,
                SegmentId = station.SegmentId.Value,
                ServiceLevelId = station.ServiceLevelId.Value,
                OperatorRoomFormatId = station.OperatorRoomFormatId.Value,
                ManagementSystemId = station.ManagementSystemId.Value,
                TradingHallOperatingModeId = station.TradingHallOperatingModeId,
                ClientRestroomId = station.ClientRestroomId,
                CashboxLocationId = station.CashboxLocationId,
                TradingHallSizeId = station.TradingHallSizeId,
                CashboxTotal = station.CashboxTotal.Value,
                ManagerArmTotal = station.ManagerArmTotal.Value,
                PersonnelPerDay = station.PersonnelPerDay.Value,
                FuelDispenserTotal = station.FuelDispenserTotal.Value,
                FuelDispenserPostTotal = station.FuelDispenserPostTotal.Value,
                FuelDispenserPostWithoutShedTotal = station.FuelDispenserPostWithoutShedTotal.Value,
                ClientRestroomTotal = station.ClientRestroomTotal.Value,
                ClientTambourTotal = station.ClientTambourTotal.Value,
                ClientSinkTotal = station.ClientSinkTotal.Value,
                TradingHallArea = station.TradingHallArea,
                CashRegisterTapeId = station.CashRegisterTapeId,
                ChequePerDay = station.ChequePerDay.Value,
                RevenueAvg = station.RevenueAvg.Value,
                HasJointRestroomEntrance = station.HasJointRestroomEntrance,
                HasSibilla = station.HasSibilla,
                HasBakery = station.HasBakery,
                HasCakes = station.HasCakes,
                DeepFryTotal = station.DeepFryTotal.Value,
                HasMarmite = station.HasMarmite,
                HasKitchen = station.HasKitchen,
                CoffeeMachineTotal = station.CoffeeMachineTotal.Value,
                DishWashingMachineTotal = station.DishWashingMachineTotal.Value,
                RepresentativenessFactor = station.RepresentativenessFactor.Value,
                RepresentativenessFactor3Quarter = station.RepresentativenessFactor3Quarter.Value,
                MerrychefTotal = station.MerrychefTotal.Value,
                DayCleaningTotal = station.DayCleaningTotal.Value,
                NightCleaningTotal = station.NightCleaningTotal.Value,
                DayRefuelingTotal = station.DayRefuelingTotal.Value,
                NightRefuelingTotal = station.NightRefuelingTotal.Value,
                HasFuelCardProgram = station.HasFuelCardProgram
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveGasStationAsync(GasStationModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                return await InsertStationAsync(model);
            }

            return await UpdateStationAsync(dbStation, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertStationAsync(GasStationModel model)
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
                    SegmentId = model.SegmentId,
                    ServiceLevelId = model.ServiceLevelId,
                    OperatorRoomFormatId = model.OperatorRoomFormatId,
                    ManagementSystemId = model.ManagementSystemId,
                    TradingHallOperatingModeId = model.TradingHallOperatingModeId,
                    ClientRestroomId = model.ClientRestroomId,
                    CashboxLocationId = model.CashboxLocationId,
                    TradingHallSizeId = model.TradingHallSizeId,
                    CashRegisterTapeId = model.CashRegisterTapeId,
                    CashboxTotal = model.CashboxTotal,
                    ManagerArmTotal = model.ManagerArmTotal,
                    PersonnelPerDay = model.PersonnelPerDay,
                    FuelDispenserTotal = model.FuelDispenserTotal,
                    FuelDispenserPostTotal = model.FuelDispenserPostTotal,
                    FuelDispenserPostWithoutShedTotal = model.FuelDispenserPostWithoutShedTotal,
                    ClientRestroomTotal = model.ClientRestroomTotal,
                    ClientTambourTotal = model.ClientTambourTotal,
                    ClientSinkTotal = model.ClientSinkTotal,
                    TradingHallArea = model.TradingHallArea,
                    ChequePerDay = model.ChequePerDay,
                    RevenueAvg = model.RevenueAvg,
                    HasJointRestroomEntrance = model.HasJointRestroomEntrance,
                    HasSibilla = model.HasSibilla,
                    HasBakery = model.HasBakery,
                    HasCakes = model.HasCakes,
                    DeepFryTotal = model.DeepFryTotal,
                    HasMarmite = model.HasMarmite,
                    HasKitchen = model.HasKitchen,
                    CoffeeMachineTotal = model.CoffeeMachineTotal,
                    DishWashingMachineTotal = model.DishWashingMachineTotal,
                    RepresentativenessFactor = model.RepresentativenessFactor,
                    RepresentativenessFactor3Quarter = model.RepresentativenessFactor3Quarter,
                    MerrychefTotal = model.MerrychefTotal,
                    DayCleaningTotal = model.DayCleaningTotal,
                    NightCleaningTotal = model.NightCleaningTotal,
                    DayRefuelingTotal = model.DayRefuelingTotal,
                    NightRefuelingTotal = model.NightRefuelingTotal,
                    HasFuelCardProgram = model.HasFuelCardProgram
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateStationAsync(GasStation station, GasStationModel model)
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
                station.SegmentId = model.SegmentId;
                station.ServiceLevelId = model.ServiceLevelId;
                station.OperatorRoomFormatId = model.OperatorRoomFormatId;
                station.ManagementSystemId = model.ManagementSystemId;
                station.TradingHallOperatingModeId = model.TradingHallOperatingModeId;
                station.ClientRestroomId = model.ClientRestroomId;
                station.CashboxLocationId = model.CashboxLocationId;
                station.TradingHallSizeId = model.TradingHallSizeId;
                station.CashboxTotal = model.CashboxTotal;
                station.ManagerArmTotal = model.ManagerArmTotal;
                station.PersonnelPerDay = model.PersonnelPerDay;
                station.FuelDispenserTotal = model.FuelDispenserTotal;
                station.FuelDispenserPostTotal = model.FuelDispenserPostTotal;
                station.FuelDispenserPostWithoutShedTotal = model.FuelDispenserPostWithoutShedTotal;
                station.ClientRestroomTotal = model.ClientRestroomTotal;
                station.ClientTambourTotal = model.ClientTambourTotal;
                station.ClientSinkTotal = model.ClientSinkTotal;
                station.TradingHallArea = model.TradingHallArea;
                station.CashRegisterTapeId = model.CashRegisterTapeId;
                station.ChequePerDay = model.ChequePerDay;
                station.RevenueAvg = model.RevenueAvg;
                station.HasJointRestroomEntrance = model.HasJointRestroomEntrance;
                station.HasSibilla = model.HasSibilla;
                station.HasBakery = model.HasBakery;
                station.HasCakes = model.HasCakes;
                station.DeepFryTotal = model.DeepFryTotal;
                station.HasMarmite = model.HasMarmite;
                station.HasKitchen = model.HasKitchen;
                station.CoffeeMachineTotal = model.CoffeeMachineTotal;
                station.DishWashingMachineTotal = model.DishWashingMachineTotal;
                station.RepresentativenessFactor = model.RepresentativenessFactor;
                station.RepresentativenessFactor3Quarter = model.RepresentativenessFactor3Quarter;
                station.MerrychefTotal = model.MerrychefTotal;
                station.DayCleaningTotal = model.DayCleaningTotal;
                station.NightCleaningTotal = model.NightCleaningTotal;
                station.DayRefuelingTotal = model.DayRefuelingTotal;
                station.NightRefuelingTotal = model.NightRefuelingTotal;
                station.HasFuelCardProgram = model.HasFuelCardProgram;

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
