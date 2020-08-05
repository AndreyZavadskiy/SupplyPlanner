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
        /// Количество касс
        /// </summary>
        public int CashboxTotal { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        public int PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество ТРК
        /// </summary>
        public int FuelDispenserTotal { get; set; }
        /// <summary>
        /// Количество сан.комнат для клиентов
        /// </summary>
        public int ClientRestroomTotal { get; set; }
        /// <summary>
        /// Площадь торгового зала
        /// </summary>
        [Column(TypeName = "decimal(8,2)")]
        public decimal TradingHallArea { get; set; }
        /// <summary>
        /// Среднее количество чеков в сутки
        /// </summary>
        public int ChequePerDay { get; set; }
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
        /// Фри
        /// </summary>
        public bool HasFrenchFry { get; set; }
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

        #region Navigation properties

        [ForeignKey("TerritoryId")]
        public RegionalStructure Territory { get; set; }
        [ForeignKey("SettlememtId")]
        public Settlement Settlement { get; set; }
        [ForeignKey("StationLocationId")]
        public StationLocation StationLocation { get; set; }
        [ForeignKey("StationStatusId")]
        public StationStatus StationStatus { get; set; }
        [ForeignKey("ServiceLevelId")]
        public ServiceLevel ServiceLevel { get; set; }
        [ForeignKey("OperatorRoomFormatId")]
        public OperatorRoomFormat OperatorRoomFormat { get; set; }
        [ForeignKey("ManagementSystemId")]
        public ManagementSystem ManagementSystem { get; set; }
        [ForeignKey("TradingHallOperatingModeId")]
        public TradingHallOperatingMode TradingHallOperatingMode { get; set; }
        [ForeignKey("ClientRestroomId")]
        public ClientRestroom ClientRestroom { get; set; }
        [ForeignKey("CashboxLocationId")]
        public CashboxLocation CashboxLocation { get; set; }
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
        /// Потребность в ТМЦ
        /// </summary>
        public ICollection<Requirement> Requirements { get; set; }

        #endregion
    }
}
