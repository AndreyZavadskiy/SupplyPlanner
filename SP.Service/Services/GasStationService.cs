﻿using System;
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
                .Include(x => x.ServiceLevel)
                .Include(x => x.OperatorRoomFormat)
                .Include(x => x.ManagementSystem)
                .Include(x => x.TradingHallOperatingMode)
                .Include(x => x.ClientRestroom)
                .Include(x => x.CashboxLocation)
                .Include(x => x.TradingHallSize)
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
                    CodeKSSS = x.CodeKSSS,
                    CodeSAP = x.CodeSAP,
                    StationNumber = x.StationNumber,
                    SettlementName = x.Settlement?.Name,
                    Address = x.Address,
                    StationLocationName = x.StationLocation?.Name,
                    StationStatusName = x.StationStatus?.Name,
                    ServiceLevelName = x.ServiceLevel?.Name,
                    OperatorRoomFormatName = x.OperatorRoomFormat?.Name,
                    ManagementSystemName = x.ManagementSystem?.Name,
                    TradingHallOperatingModeName = x.TradingHallOperatingMode?.Name,
                    ClientRestroomName = x.ClientRestroom?.Name,
                    CashboxLocationName = x.CashboxLocation?.Name,
                    TradingHallSizeName = x.TradingHallSize?.Name,
                    CashboxTotal = x.CashboxTotal,
                    PersonnelPerDay = x.PersonnelPerDay,
                    FuelDispenserTotal = x.FuelDispenserTotal,
                    FuelDispenserPostTotal = x.FuelDispenserPostTotal,
                    ClientRestroomTotal = x.ClientRestroomTotal,
                    ClientTambourTotal = x.ClientTambourTotal,
                    ClientSinkTotal = x.ClientSinkTotal,
                    TradingHallArea = x.TradingHallArea,
                    ChequePerDay = x.ChequePerDay,
                    RevenueAvg = x.RevenueAvg,
                    HasSibilla = x.HasSibilla,
                    HasBakery = x.HasBakery,
                    HasCakes = x.HasCakes,
                    DeepFryTotal = x.DeepFryTotal,
                    HasMarmite = x.HasMarmite,
                    HasKitchen = x.HasKitchen,
                    CoffeeMachineTotal = x.CoffeeMachineTotal,
                    DishWashingMachineTotal = x.DishWashingMachineTotal,
                    RepresentativenessFactor = x.RepresentativenessFactor,
                    RepresentativenessFactor3Quarter = x.RepresentativenessFactor3Quarter,
                    MerrychefTotal = x.MerrychefTotal,
                    DayCleaningTotal = x.DayCleaningTotal,
                    NightCleaningTotal = x.NightCleaningTotal
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
                FuelDispenserPostTotal = station.FuelDispenserPostTotal,
                ClientRestroomTotal = station.ClientRestroomTotal,
                ClientTambourTotal = station.ClientTambourTotal,
                ClientSinkTotal = station.ClientSinkTotal,
                TradingHallArea = station.TradingHallArea,
                ChequePerDay = station.ChequePerDay,
                RevenueAvg = station.RevenueAvg,
                HasSibilla = station.HasSibilla,
                HasBakery = station.HasBakery,
                HasCakes = station.HasCakes,
                DeepFryTotal = station.DeepFryTotal,
                HasMarmite = station.HasMarmite,
                HasKitchen = station.HasKitchen,
                CoffeeMachineTotal = station.CoffeeMachineTotal,
                DishWashingMachineTotal = station.DishWashingMachineTotal,
                RepresentativenessFactor = station.RepresentativenessFactor,
                RepresentativenessFactor3Quarter = station.RepresentativenessFactor3Quarter,
                MerrychefTotal = station.MerrychefTotal,
                DayCleaningTotal = station.DayCleaningTotal,
                NightCleaningTotal = station.NightCleaningTotal
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
                    FuelDispenserPostTotal = model.FuelDispenserPostTotal,
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
                    NightCleaningTotal = model.NightCleaningTotal
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
                station.FuelDispenserPostTotal = model.FuelDispenserPostTotal;
                station.ClientRestroomTotal = model.ClientRestroomTotal;
                station.ClientTambourTotal = model.ClientTambourTotal;
                station.ClientSinkTotal = model.ClientSinkTotal;
                station.TradingHallArea = model.TradingHallArea;
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
