using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        Task<(bool Success, int? Id, IEnumerable<string> Errors, string Previous, string Next)> SaveGasStationAsync(GasStationModel model);

        Task<IEnumerable<FuelBaseListItem>> GetFuelBaseListAsync();
        Task<IEnumerable<FuelBaseListItem>> GetFuelBaseListAsync(int? personId);
        Task<FuelBaseModel> GetFuelBaseAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveFuelBaseAsync(FuelBaseModel model);

        Task<IEnumerable<OfficeListItem>> GetOfficeListAsync();
        Task<IEnumerable<OfficeListItem>> GetOfficeListAsync(int? personId);
        Task<OfficeModel> GetOfficeAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveOfficeAsync(OfficeModel model);

        Task<IEnumerable<LaboratoryListItem>> GetLaboratoryListAsync();
        Task<IEnumerable<LaboratoryListItem>> GetLaboratoryListAsync(int? personId);
        Task<LaboratoryModel> GetLaboratoryAsync(int id);
        Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveLaboratoryAsync(LaboratoryModel model);
    }

    class ChangeItem
    {
        public string PreviousValue {  get; set; }
        public string NextValue { get; set; }
        public string Name { get; set; }
    }

    public class GasStationService : IGasStationService
    {
        private readonly ApplicationDbContext _context;

        public GasStationService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Gas stations

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
                query = query.Where(x => regionTerritories.Contains(x.TerritoryId.Value));
            }
            if (territories != null)
            {
                query = query.Where(x => territories.Contains(x.TerritoryId.Value));
            }
            else if (personId != null)
            {
                var managerTerritories = await _context.PersonTerritories
                    .Where(x => x.PersonId == personId)
                    .Select(x => x.RegionalStructureId)
                    .Distinct()
                    .ToArrayAsync();
                query = query.Where(x => managerTerritories.Contains(x.TerritoryId.Value));
            }

            var stations = query.AsEnumerable()
                .Select(x => new GasStationListItem
                {
                    Id = x.Id,
                    RegionId = x.Territory.ParentId.Value,
                    RegionName = x.Territory.Parent?.Name,
                    TerritoryId = x.TerritoryId.Value,
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
                .Where(x => x.ObjectType == Core.Enum.ObjectType.GasStation)
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
                TerritoryId = station.TerritoryId.Value,
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

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors, string Previous, string Next)> SaveGasStationAsync(GasStationModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                var insertResult = await InsertStationAsync(model);
                return (insertResult.Success, insertResult.Id, insertResult.Errors, null, null);
            }

            var updateResult = await UpdateStationAsync(dbStation, model);
            if (updateResult.Changes == null || updateResult.Changes.Count() == 0)
            {
                return (updateResult.Success, updateResult.Id, updateResult.Errors, null, null);
            }

            string previous = string.Join("; ", updateResult.Changes.Select(x => $"{x.Name}: {x.PreviousValue}"));
            string next = string.Join("; ", updateResult.Changes.Select(x => $"{x.Name}: {x.NextValue}"));
            return (updateResult.Success, updateResult.Id, updateResult.Errors, previous, next);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors, List<ChangeItem> Changes)> InsertStationAsync(GasStationModel model)
        {
            var errors = new List<string>();
            try
            {
                var gasStation = new GasStation
                {
                    ObjectType = Core.Enum.ObjectType.GasStation,
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

                return (true, gasStation.Id, null, null);
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

            return (false, null, errors, null);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors, List<ChangeItem> Changes)> UpdateStationAsync(GasStation station, GasStationModel model)
        {
            var errors = new List<string>();

            var changes = GetChanges(station, model);

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

                return (true, model.Id, null, changes);
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

            return (false, model.Id, errors, null);
        }

        private List<ChangeItem> GetChanges(GasStation station, GasStationModel model)
        {
            var changeList = new List<ChangeItem>();

            GetFieldChanges(changeList, station.Code, model.Code, "Код");
            GetFieldChanges(changeList, station.CodeKSSS, model.CodeKSSS, "Код КССС");
            GetFieldChanges(changeList, station.CodeSAP, model.CodeSAP, "Код SAP");
            GetFieldChanges(changeList, station.StationNumber, model.StationNumber, "Номер АЗС");
            GetFieldChanges(changeList, station.TerritoryId, model.TerritoryId, "Территория");
            GetFieldChanges(changeList, station.SettlementId, model.SettlementId, "Населенный пункт");
            GetFieldChanges(changeList, station.Address, model.Address, "Адрес");
            GetFieldChanges(changeList, station.StationLocationId, model.StationLocationId, "Месторасположение");
            GetFieldChanges(changeList, station.StationStatusId, model.StationStatusId, "Статус");
            GetFieldChanges(changeList, station.SegmentId, model.SegmentId, "Сегмент");
            GetFieldChanges(changeList, station.ServiceLevelId, model.ServiceLevelId, "Кластер (уровень сервиса)");
            GetFieldChanges(changeList, station.OperatorRoomFormatId, model.OperatorRoomFormatId, "Формат операторной");
            GetFieldChanges(changeList, station.ManagementSystemId, model.ManagementSystemId, "Система управления");
            GetFieldChanges(changeList, station.TradingHallOperatingModeId, model.TradingHallOperatingModeId, "Режим работы торгового зала");
            GetFieldChanges(changeList, station.ClientRestroomId, model.ClientRestroomId, "Санузел для клиентов");
            GetFieldChanges(changeList, station.CashboxLocationId, model.CashboxLocationId, "Расчетно-кассовый узел");
            GetFieldChanges(changeList, station.TradingHallSizeId, model.TradingHallSizeId, "Размер торгового зала");
            GetFieldChanges(changeList, station.CashboxTotal, model.CashboxTotal, "Количество АРМ (касс)");
            GetFieldChanges(changeList, station.ManagerArmTotal, model.ManagerArmTotal, "Количество АРМ менеджера");
            GetFieldChanges(changeList, station.PersonnelPerDay, model.PersonnelPerDay, "Количество персонала в сутки");
            GetFieldChanges(changeList, station.FuelDispenserTotal, model.FuelDispenserTotal, "Количество ТРК");
            GetFieldChanges(changeList, station.FuelDispenserPostTotal, model.FuelDispenserPostTotal, "Количество постов ТРК");
            GetFieldChanges(changeList, station.FuelDispenserPostWithoutShedTotal, model.FuelDispenserPostWithoutShedTotal, "Количество постов ТРК без навеса");
            GetFieldChanges(changeList, station.ClientRestroomTotal, model.ClientRestroomTotal, "Количество сан.комнат для клиентов");
            GetFieldChanges(changeList, station.ClientTambourTotal, model.ClientTambourTotal, "Количество тамбуров для клиентов");
            GetFieldChanges(changeList, station.ClientSinkTotal, model.ClientSinkTotal, "Количество раковин для клиентов");
            GetFieldChanges(changeList, station.TradingHallArea, model.TradingHallArea, "Площадь торгового зала");
            GetFieldChanges(changeList, station.CashRegisterTapeId, model.CashRegisterTapeId, "Вид термоленты");
            GetFieldChanges(changeList, station.ChequePerDay, model.ChequePerDay, "Среднее количество чеков в сутки");
            GetFieldChanges(changeList, station.RevenueAvg, model.RevenueAvg, "Выручка в месяц");
            GetFieldChanges(changeList, station.HasJointRestroomEntrance, model.HasJointRestroomEntrance, "Общий тамбур с раковиной");
            GetFieldChanges(changeList, station.HasSibilla, model.HasSibilla, "Сибилла");
            GetFieldChanges(changeList, station.HasBakery, model.HasBakery, "Выпечка");
            GetFieldChanges(changeList, station.HasCakes, model.HasCakes, "Торты");
            GetFieldChanges(changeList, station.DeepFryTotal, model.DeepFryTotal, "Количество фритюрных аппаратов");
            GetFieldChanges(changeList, station.HasMarmite, model.HasMarmite, "Мармит");
            GetFieldChanges(changeList, station.HasKitchen, model.HasKitchen, "Кухня");
            GetFieldChanges(changeList, station.CoffeeMachineTotal, model.CoffeeMachineTotal, "Количество кофемашин");
            GetFieldChanges(changeList, station.DishWashingMachineTotal, model.DishWashingMachineTotal, "Количество посудомоечных машин");
            GetFieldChanges(changeList, station.RepresentativenessFactor, model.RepresentativenessFactor, "Имиджевый коэффициент");
            GetFieldChanges(changeList, station.RepresentativenessFactor3Quarter, model.RepresentativenessFactor3Quarter, "Имиджевый коэффициент 3 кв.");
            GetFieldChanges(changeList, station.MerrychefTotal, model.MerrychefTotal, "Количество печей Меришеф");
            GetFieldChanges(changeList, station.DayCleaningTotal, model.DayCleaningTotal, "Уборка в день");
            GetFieldChanges(changeList, station.NightCleaningTotal, model.NightCleaningTotal, "Уборка в ночь");
            GetFieldChanges(changeList, station.DayRefuelingTotal, model.DayRefuelingTotal, "Расстановка заправки в день");
            GetFieldChanges(changeList, station.NightRefuelingTotal, model.NightRefuelingTotal, "Расстановка заправки в ночь");
            GetFieldChanges(changeList, station.HasFuelCardProgram, model.HasFuelCardProgram, "Топливные карты");

            return changeList;
        }

        private void GetFieldChanges(List<ChangeItem> changeList, string prev, string next, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(prev))
                prev = string.Empty;
            if (string.IsNullOrWhiteSpace(next))
                next = string.Empty;
            if (prev == next)
                return;

            changeList.Add(new ChangeItem { 
                PreviousValue = prev,
                NextValue = next,
                Name = fieldName,
            });
        }

        private void GetFieldChanges(List<ChangeItem> changeList, int prev, int next, string fieldName)
        {
            GetFieldChanges(changeList, prev.ToString(), next.ToString(), fieldName);
        }

        private void GetFieldChanges(List<ChangeItem> changeList, int? prev, int? next, string fieldName)
        {
            GetFieldChanges(changeList, prev?.ToString(), next?.ToString(), fieldName);
        }

        private void GetFieldChanges(List<ChangeItem> changeList, decimal prev, decimal next, string fieldName)
        {
            GetFieldChanges(changeList, prev.ToString(), next.ToString(), fieldName);
        }

        private void GetFieldChanges(List<ChangeItem> changeList, decimal? prev, decimal? next, string fieldName)
        {
            GetFieldChanges(changeList, prev?.ToString(), next?.ToString(), fieldName);
        }

        private void GetFieldChanges(List<ChangeItem> changeList, bool prev, bool next, string fieldName)
        {
            string prevText = prev ? "да" : "нет";
            string nextText = next ? "да" : "нет";
            GetFieldChanges(changeList, prevText, nextText, fieldName);
        }

        #endregion

        #region Fuel bases

        public async Task<IEnumerable<FuelBaseListItem>> GetFuelBaseListAsync()
        {
            return await GetFuelBaseListAsync(null);
        }

        public async Task<IEnumerable<FuelBaseListItem>> GetFuelBaseListAsync(int? personId)
        {
            var query = _context.GasStations.AsNoTracking()
                .Where(x => x.ObjectType == Core.Enum.ObjectType.FuelBase)
                .AsQueryable();
            if (personId != null)
            {
                // TODO: сделать управление правами
            }

            var stations = query.AsEnumerable()
                .Select(x => new FuelBaseListItem
                {
                    Id = x.Id,
                    ObjectName = x.ObjectName,
                    PersonnelTotal = x.PersonnelTotal,
                    ShiftPerDay = x.ShiftPerDay,
                    PersonnelPerShift = x.PersonnelPerShift,
                    PersonnelPerDay = x.PersonnelPerDay,
                    FlagpoleTotal = x.FlagpoleTotal,
                    RailwayDeliveryPlanTotal = x.RailwayDeliveryPlanTotal,
                    FuelTrackPerYear = x.FuelTrackPerYear,
                    RailwayTankPerYear = x.RailwayTankPerYear,
                    ReservoirTotal = x.ReservoirTotal,
                    WorkingPlaceTotal = x.WorkingPlaceTotal,
                    RestroomTotal = x.RestroomTotal,
                    Fuel92PerYear = x.Fuel92PerYear,
                    Fuel95PerYear = x.Fuel95PerYear,
                    Fuel100PerYear = x.Fuel100PerYear,
                    DieselFuelPerYear = x.DieselFuelPerYear,
                    HasFuelBaseAutomation = x.HasFuelBaseAutomation,
                    AntiIcingSquare = x.AntiIcingSquare,
                    AntiIcingPerYear = x.AntiIcingPerYear,
                    DiningRoomTotal = x.DiningRoomTotal
                })
                .ToArray();

            return stations;
        }

        public async Task<FuelBaseModel> GetFuelBaseAsync(int id)
        {
            var station = await _context.GasStations.FindAsync(id);

            if (station == null)
            {
                return null;
            }

            var model = new FuelBaseModel
            {
                Id = station.Id,
                ObjectName = station.ObjectName,
                Address = station.Address,
                PersonnelTotal = station.PersonnelTotal,
                ShiftPerDay = station.ShiftPerDay,
                PersonnelPerShift = station.PersonnelPerShift,
                PersonnelPerDay = station.PersonnelPerDay,
                FlagpoleTotal = station.FlagpoleTotal,
                RailwayDeliveryPlanTotal = station.RailwayDeliveryPlanTotal,
                FuelTrackPerYear = station.FuelTrackPerYear,
                RailwayTankPerYear = station.RailwayTankPerYear,
                ReservoirTotal = station.ReservoirTotal,
                WorkingPlaceTotal = station.WorkingPlaceTotal,
                RestroomTotal = station.RestroomTotal,
                Fuel92PerYear = station.Fuel92PerYear,
                Fuel95PerYear = station.Fuel95PerYear,
                Fuel100PerYear = station.Fuel100PerYear,
                DieselFuelPerYear = station.DieselFuelPerYear,
                HasFuelBaseAutomation = station.HasFuelBaseAutomation,
                AntiIcingSquare = station.AntiIcingSquare,
                AntiIcingPerYear = station.AntiIcingPerYear,
                DiningRoomTotal = station.DiningRoomTotal
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveFuelBaseAsync(FuelBaseModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                return await InsertFuelBaseAsync(model);
            }

            return await UpdateFuelBaseAsync(dbStation, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertFuelBaseAsync(FuelBaseModel model)
        {
            var errors = new List<string>();
            try
            {
                var gasStation = new GasStation
                {
                    ObjectType = Core.Enum.ObjectType.FuelBase,
                    ObjectName = model.ObjectName,
                    Address = model.Address,
                    PersonnelTotal = model.PersonnelTotal,
                    ShiftPerDay = model.ShiftPerDay,
                    PersonnelPerShift = model.PersonnelPerShift,
                    PersonnelPerDay = model.PersonnelPerDay,
                    FlagpoleTotal = model.FlagpoleTotal,
                    RailwayDeliveryPlanTotal = model.RailwayDeliveryPlanTotal,
                    FuelTrackPerYear = model.FuelTrackPerYear,
                    RailwayTankPerYear = model.RailwayTankPerYear,
                    ReservoirTotal = model.ReservoirTotal,
                    WorkingPlaceTotal = model.WorkingPlaceTotal,
                    RestroomTotal = model.RestroomTotal,
                    Fuel92PerYear = model.Fuel92PerYear,
                    Fuel95PerYear = model.Fuel95PerYear,
                    Fuel100PerYear = model.Fuel100PerYear,
                    DieselFuelPerYear = model.DieselFuelPerYear,
                    HasFuelBaseAutomation = model.HasFuelBaseAutomation,
                    AntiIcingSquare = model.AntiIcingSquare,
                    AntiIcingPerYear = model.AntiIcingPerYear,
                    DiningRoomTotal = model.DiningRoomTotal
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateFuelBaseAsync(GasStation station, FuelBaseModel model)
        {
            var errors = new List<string>();

            try
            {
                station.ObjectName = model.ObjectName;
                station.Address = model.Address;
                station.PersonnelTotal = model.PersonnelTotal;
                station.ShiftPerDay = model.ShiftPerDay;
                station.PersonnelPerShift = model.PersonnelPerShift;
                station.PersonnelPerDay = model.PersonnelPerDay;
                station.FlagpoleTotal = model.FlagpoleTotal;
                station.RailwayDeliveryPlanTotal = model.RailwayDeliveryPlanTotal;
                station.FuelTrackPerYear = model.FuelTrackPerYear;
                station.RailwayTankPerYear = model.RailwayTankPerYear;
                station.ReservoirTotal = model.ReservoirTotal;
                station.WorkingPlaceTotal = model.WorkingPlaceTotal;
                station.RestroomTotal = model.RestroomTotal;
                station.Fuel92PerYear = model.Fuel92PerYear;
                station.Fuel95PerYear = model.Fuel95PerYear;
                station.Fuel100PerYear = model.Fuel100PerYear;
                station.DieselFuelPerYear = model.DieselFuelPerYear;
                station.HasFuelBaseAutomation = model.HasFuelBaseAutomation;
                station.AntiIcingSquare = model.AntiIcingSquare;
                station.AntiIcingPerYear = model.AntiIcingPerYear;
                station.DiningRoomTotal = model.DiningRoomTotal;

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

        #endregion

        #region Offices

        public async Task<IEnumerable<OfficeListItem>> GetOfficeListAsync()
        {
            return await GetOfficeListAsync(null);
        }

        public async Task<IEnumerable<OfficeListItem>> GetOfficeListAsync(int? personId)
        {
            var query = _context.GasStations.AsNoTracking()
                .Where(x => x.ObjectType == Core.Enum.ObjectType.Office)
                .AsQueryable();
            if (personId != null)
            {
                // TODO: сделать управление правами
            }

            var stations = query.AsEnumerable()
                .Select(x => new OfficeListItem
                {
                    Id = x.Id,
                    ObjectName = x.ObjectName,
                    PersonnelTotal = x.PersonnelTotal,
                    DepartmentTotal = x.DepartmentTotal,
                    FlagpoleTotal = x.FlagpoleTotal,
                    HasCentralWaterSupply = x.HasCentralWaterSupply,
                })
                .ToArray();

            return stations;
        }

        public async Task<OfficeModel> GetOfficeAsync(int id)
        {
            var station = await _context.GasStations.FindAsync(id);

            if (station == null)
            {
                return null;
            }

            var model = new OfficeModel
            {
                Id = station.Id,
                ObjectName = station.ObjectName,
                Address = station.Address,
                PersonnelTotal = station.PersonnelTotal,
                DepartmentTotal = station.DepartmentTotal,
                FlagpoleTotal = station.FlagpoleTotal,
                HasCentralWaterSupply = station.HasCentralWaterSupply,
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveOfficeAsync(OfficeModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                return await InsertOfficeAsync(model);
            }

            return await UpdateOfficeAsync(dbStation, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertOfficeAsync(OfficeModel model)
        {
            var errors = new List<string>();
            try
            {
                var gasStation = new GasStation
                {
                    ObjectType = Core.Enum.ObjectType.Office,
                    ObjectName = model.ObjectName,
                    Address = model.Address,
                    PersonnelTotal = model.PersonnelTotal,
                    DepartmentTotal = model.DepartmentTotal,
                    FlagpoleTotal = model.FlagpoleTotal,
                    HasCentralWaterSupply = model.HasCentralWaterSupply,
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateOfficeAsync(GasStation station, OfficeModel model)
        {
            var errors = new List<string>();

            try
            {
                station.ObjectName = model.ObjectName;
                station.ObjectName = model.ObjectName;
                station.Address = model.Address;
                station.PersonnelTotal = model.PersonnelTotal;
                station.DepartmentTotal = model.DepartmentTotal;
                station.FlagpoleTotal = model.FlagpoleTotal;
                station.HasCentralWaterSupply = model.HasCentralWaterSupply;

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

        #endregion

        #region Laboratories

        public async Task<IEnumerable<LaboratoryListItem>> GetLaboratoryListAsync()
        {
            return await GetLaboratoryListAsync(null);
        }

        public async Task<IEnumerable<LaboratoryListItem>> GetLaboratoryListAsync(int? personId)
        {
            var query = _context.GasStations.AsNoTracking()
                .Where(x => x.ObjectType == Core.Enum.ObjectType.Laboratory)
                .AsQueryable();
            if (personId != null)
            {
                // TODO: сделать управление правами
            }

            var stations = query.AsEnumerable()
                .Select(x => new LaboratoryListItem
                {
                    Id = x.Id,
                    ObjectName = x.ObjectName,
                    PersonnelTotal = x.PersonnelTotal,
                    ServicingGasStationTotal = x.ServicingGasStationTotal,
                    AverageTestPerMonth = x.AverageTestPerMonth,
                    WorkingRoomTotal = x.WorkingRoomTotal,
                    HasWell = x.HasWell,
                    DiningRoomTotal = x.DiningRoomTotal,
                    HasSpectroscan = x.HasSpectroscan,
                    HasSindyAnalyzer = x.HasSindyAnalyzer,
                    RestroomTotal = x.RestroomTotal,
                    StampTotal = x.StampTotal,
                    SpecialistTotalForElectricalTest = x.SpecialistTotalForElectricalTest,
                    ElectricalTestPerYear = x.ElectricalTestPerYear,
                    LaboratoryWorkSchedule = x.LaboratoryWorkSchedule,
                })
                .ToArray();

            return stations;
        }

        public async Task<LaboratoryModel> GetLaboratoryAsync(int id)
        {
            var station = await _context.GasStations.FindAsync(id);

            if (station == null)
            {
                return null;
            }

            var model = new LaboratoryModel
            {
                Id = station.Id,
                ObjectName = station.ObjectName,
                Address = station.Address,
                PersonnelTotal = station.PersonnelTotal,
                ServicingGasStationTotal = station.ServicingGasStationTotal,
                AverageTestPerMonth = station.AverageTestPerMonth,
                WorkingRoomTotal = station.WorkingRoomTotal,
                HasWell = station.HasWell,
                DiningRoomTotal = station.DiningRoomTotal,
                HasSpectroscan = station.HasSpectroscan,
                HasSindyAnalyzer = station.HasSindyAnalyzer,
                RestroomTotal = station.RestroomTotal,
                StampTotal = station.StampTotal,
                SpecialistTotalForElectricalTest = station.SpecialistTotalForElectricalTest,
                ElectricalTestPerYear = station.ElectricalTestPerYear,
                LaboratoryWorkSchedule = station.LaboratoryWorkSchedule,
            };

            return model;
        }

        public async Task<(bool Success, int? Id, IEnumerable<string> Errors)> SaveLaboratoryAsync(LaboratoryModel model)
        {
            var dbStation = await _context.GasStations.FindAsync(model.Id);
            if (dbStation == null)
            {
                return await InsertLaboratoryAsync(model);
            }

            return await UpdateLaboratoryAsync(dbStation, model);
        }

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> InsertLaboratoryAsync(LaboratoryModel model)
        {
            var errors = new List<string>();
            try
            {
                var gasStation = new GasStation
                {
                    ObjectType = Core.Enum.ObjectType.Laboratory,
                    ObjectName = model.ObjectName,
                    Address = model.Address,
                    PersonnelTotal = model.PersonnelTotal,
                    ServicingGasStationTotal = model.ServicingGasStationTotal,
                    AverageTestPerMonth = model.AverageTestPerMonth,
                    WorkingRoomTotal = model.WorkingRoomTotal,
                    HasWell = model.HasWell,
                    DiningRoomTotal = model.DiningRoomTotal,
                    HasSpectroscan = model.HasSpectroscan,
                    HasSindyAnalyzer = model.HasSindyAnalyzer,
                    RestroomTotal = model.RestroomTotal,
                    StampTotal = model.StampTotal,
                    SpecialistTotalForElectricalTest = model.SpecialistTotalForElectricalTest,
                    ElectricalTestPerYear = model.ElectricalTestPerYear,
                    LaboratoryWorkSchedule = model.LaboratoryWorkSchedule,
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

        private async Task<(bool Success, int? Id, IEnumerable<string> Errors)> UpdateLaboratoryAsync(GasStation station, LaboratoryModel model)
        {
            var errors = new List<string>();

            try
            {
                station.ObjectName = model.ObjectName;
                station.ObjectName = model.ObjectName;
                station.Address = model.Address;
                station.PersonnelTotal = model.PersonnelTotal;
                station.ServicingGasStationTotal = model.ServicingGasStationTotal;
                station.AverageTestPerMonth = model.AverageTestPerMonth;
                station.WorkingRoomTotal = model.WorkingRoomTotal;
                station.HasWell = model.HasWell;
                station.DiningRoomTotal = model.DiningRoomTotal;
                station.HasSpectroscan = model.HasSpectroscan;
                station.HasSindyAnalyzer = model.HasSindyAnalyzer;
                station.RestroomTotal = model.RestroomTotal;
                station.StampTotal = model.StampTotal;
                station.SpecialistTotalForElectricalTest = model.SpecialistTotalForElectricalTest;
                station.ElectricalTestPerYear = model.ElectricalTestPerYear;
                station.LaboratoryWorkSchedule = model.LaboratoryWorkSchedule;

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

        #endregion
    }
}
