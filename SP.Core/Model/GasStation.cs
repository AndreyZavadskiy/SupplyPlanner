using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Master;

namespace SP.Core.Model
{
    /// <summary>
    /// АЗС
    /// </summary>
    [Table("GasStation")]
    public class GasStation
    {
        /// <summary>
        ///  ID АЗС
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// Код КССС
        /// </summary>
        public int CodeKSSS { get; set; }
        /// <summary>
        /// Код SAP R/3
        /// </summary>
        [StringLength(20)]
        public string CodeSAP { get; set; }
        /// <summary>
        /// Номер АЗС
        /// </summary>
        [StringLength(5)]
        public string StationNumber { get; set; }
        /// <summary>
        /// ID территории
        /// </summary>
        public int TerritoryId { get; set; }
        /// <summary>
        /// ID населенного пункта
        /// </summary>
        public int SettlementId { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        [StringLength(200)]
        public string Address { get; set; }
        /// <summary>
        /// Местоположение
        /// </summary>
        public int StationLocationId { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public int StationStatusId { get; set; }
        /// <summary>
        /// Сегмент
        /// </summary>
        public int SegmentId { get; set; }
        /// <summary>
        /// Кластер (уровень сервиса)
        /// </summary>
        public int ServiceLevelId { get; set; }
        /// <summary>
        /// Формат операторной
        /// </summary>
        public int OperatorRoomFormatId { get; set; }
        /// <summary>
        /// Система управления
        /// </summary>
        public int ManagementSystemId { get; set; }
        /// <summary>
        /// Режим работы торгового зала
        /// не ведется для ААЗС и окон
        /// </summary>
        public int? TradingHallOperatingModeId { get; set; }
        /// <summary>
        /// Санузел для клиентов
        /// не ведется для ААЗС
        /// </summary>
        public int? ClientRestroomId { get; set; }
        /// <summary>
        /// Расчетно-кассовый узел
        /// не ведется для ААЗС и окон
        /// </summary>
        public int? CashboxLocationId { get; set; }
        /// <summary>
        /// Размер торгового зала
        /// не ведется для ААЗС и окон
        /// </summary>
        public int? TradingHallSizeId { get; set; }
        /// <summary>
        /// Вид термоленты
        /// </summary>
        public int? CashRegisterTapeId { get; set; }
        /// <summary>
        /// Количество АРМ/касс
        /// </summary>
        public int CashboxTotal { get; set; }
        /// <summary>
        /// Количество АРМ менеджера
        /// </summary>
        public int ManagerArmTotal { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        [Column(TypeName = "decimal(8,2)")] 
        public decimal PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество ТРК
        /// </summary>
        public int FuelDispenserTotal { get; set; }
        /// <summary>
        /// Количество постов ТРК
        /// </summary>
        public int FuelDispenserPostTotal { get; set; }
        /// <summary>
        /// Количество постов ТРК без навеса
        /// </summary>
        public int FuelDispenserPostWithoutShedTotal { get; set; }
        /// <summary>
        /// Количество сан.комнат для клиентов
        /// </summary>
        public int ClientRestroomTotal { get; set; }
        /// <summary>
        /// Количество тамбуров для клиентов
        /// </summary>
        public int ClientTambourTotal { get; set; }
        /// <summary>
        /// Количество раковин для клиентов
        /// </summary>
        public int ClientSinkTotal { get; set; }
        /// <summary>
        /// Площадь торгового зала
        /// не ведется для ААЗС и окон
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        public decimal? TradingHallArea { get; set; }
        /// <summary>
        /// Среднее количество чеков в сутки
        /// </summary>
        public decimal ChequePerDay { get; set; }
        /// <summary>
        /// Выручка в месяц, руб.
        /// </summary>
        [Column(TypeName = "decimal(12,2)")]
        public decimal RevenueAvg { get; set; }
        /// <summary>
        /// Общий тамбур с раковиной
        /// </summary>
        public bool HasJointRestroomEntrance { get; set; }
        /// <summary>
        /// Сибилла
        /// </summary>
        public bool HasSibilla { get; set; }
        /// <summary>
        /// Выпечка
        /// </summary>
        public bool HasBakery { get; set; }
        /// <summary>
        /// Торты
        /// </summary>
        public bool HasCakes { get; set; }
        /// <summary>
        /// Количество фритюрных аппаратов
        /// </summary>
        public int DeepFryTotal { get; set; }
        /// <summary>
        /// Мармит
        /// </summary>
        public bool HasMarmite { get; set; }
        /// <summary>
        /// Кухня
        /// </summary>
        public bool HasKitchen { get; set; }
        /// <summary>
        /// Количество кофеаппаратов на жидком молоке
        /// </summary>
        public int CoffeeMachineTotal { get; set; }
        /// <summary>
        /// Количество посудомоечных машин
        /// </summary>
        public int DishWashingMachineTotal { get; set; }
        /// <summary>
        /// Расход кассовой ленты за день, м
        /// кол-во транзакций * среднюю длину чека
        /// </summary>
        [Obsolete]
        [Column(TypeName = "decimal(8,2)")]
        public decimal ChequeBandLengthPerDay { get; set; }
        /// <summary>
        /// Имиджевый коэффициент
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        public decimal RepresentativenessFactor { get; set; }
        /// <summary>
        /// Имиджевый коэффициент 3 квартал
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        public decimal RepresentativenessFactor3Quarter { get; set; }
        /// <summary>
        /// Количество комбипечей Меришеф
        /// </summary>
        public int MerrychefTotal { get; set; }
        /// <summary>
        /// Уборка в день
        /// </summary>
        public int DayCleaningTotal { get; set; }
        /// <summary>
        /// Уборка в ночь
        /// </summary>
        public int NightCleaningTotal { get; set; }
        /// <summary>
        /// Расстановка заправки в день
        /// </summary>
        public int DayRefuelingTotal { get; set; }
        /// <summary>
        /// Расстановка заправки в ночь
        /// </summary>
        public int NightRefuelingTotal { get; set; }
        /// <summary>
        /// Участвует в проекте выдачи топливных карт
        /// </summary>
        public bool HasFuelCardProgram { get; set; }

        #region Navigation properties

        [ForeignKey("CashboxLocationId")]
        public CashboxLocation CashboxLocation { get; set; }
        [ForeignKey("CashRegisterTapeId")]
        public CashRegisterTape CashRegisterTape { get; set; }
        [ForeignKey("ClientRestroomId")]
        public ClientRestroom ClientRestroom { get; set; }
        [ForeignKey("ManagementSystemId")]
        public ManagementSystem ManagementSystem { get; set; }
        [ForeignKey("SegmentId")]
        public Segment Segment { get; set; }
        [ForeignKey("OperatorRoomFormatId")]
        public OperatorRoomFormat OperatorRoomFormat { get; set; }
        [ForeignKey("SettlememtId")]
        public Settlement Settlement { get; set; }
        [ForeignKey("StationLocationId")]
        public StationLocation StationLocation { get; set; }
        [ForeignKey("StationStatusId")]
        public StationStatus StationStatus { get; set; }
        [ForeignKey("ServiceLevelId")]
        public ServiceLevel ServiceLevel { get; set; }
        [ForeignKey("TerritoryId")]
        public RegionalStructure Territory { get; set; }
        [ForeignKey("TradingHallOperatingModeId")]
        public TradingHallOperatingMode TradingHallOperatingMode { get; set; }
        [ForeignKey("TradingHallSizeId")]
        public TradingHallSize TradingHallSize { get; set; }

        /// <summary>
        /// Выгрузка ТМЦ
        /// </summary>
        public ICollection<StageInventory> StageInventories { get; set; }
        /// <summary>
        /// ТМЦ, состыкованные с Номенклатурой
        /// </summary>
        public ICollection<Inventory> Inventories { get; set; }
        /// <summary>
        /// Остатки и потребность по Номенклатуре
        /// </summary>
        public ICollection<CalcSheet> CalcSheets { get; set; }

        #endregion
    }
}
