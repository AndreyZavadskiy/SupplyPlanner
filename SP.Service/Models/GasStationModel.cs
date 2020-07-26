using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    public class GasStationModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        public int Id { get; set; }
        /// <summary>
        /// Код АЗС
        /// </summary>
        [DisplayName("Код")]
        public string Code { get; set; }
        /// <summary>
        /// Код КССС
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Код КССС")]
        public int CodeKSSS { get; set; }
        /// <summary>
        /// Код SAP
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Код SAP")]
        public string CodeSAP { get; set; }
        /// <summary>
        /// Номер АЗС
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Номер АЗС")]
        public string StationNumber { get; set; }
        /// <summary>
        /// ID региона
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Регион")]
        public int RegionId { get; set; }
        /// <summary>
        /// ID территории
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Территория")]
        public int TerritoryId { get; set; }
        /// <summary>
        /// Населенный пункт
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Населенный пункт")]
        public int SettlementId { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Адрес")]
        public string Address { get; set; }
        /// <summary>
        /// Месторасположение
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Месторасположение")]
        public int StationLocationId { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Статус")]
        public int StationStatusId { get; set; }
        /// <summary>
        /// Кластер (уровень сервиса)
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Кластер (уровень сервиса)")]
        public int ServiceLevelId { get; set; }
        /// <summary>
        /// Формат операторной
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Формат операторной")]
        public int OperatorRoomFormatId { get; set; }
        /// <summary>
        /// Система управления
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Система управления")]
        public int ManagementSystemId { get; set; }
        /// <summary>
        /// Режим работы торгового зала
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Режим работы торгового зала")]
        public int TradingHallOperatingModeId { get; set; }
        /// <summary>
        /// Санузел для клиентов
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Санузел для клиентов")]
        public int ClientRestroomId { get; set; }
        /// <summary>
        /// Расчетно-кассовый узел
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Расчетно-кассовый узел")]
        public int CashboxLocationId { get; set; }
        /// <summary>
        /// Размер торгового зала
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Размер торгового зала")]
        public int TradingHallSizeId { get; set; }
        /// <summary>
        /// Количество касс
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество касс")]
        public int CashboxTotal { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество персонала в сутки")]
        public int PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество ТРК
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество ТРК")]
        public int FuelDispenserTotal { get; set; }
        /// <summary>
        /// Количество сан.комнат для клиентов
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество сан.комнат для клиентов")]
        public int ClientRestroomTotal { get; set; }
        /// <summary>
        /// Площадь торгового зала
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Площадь торгового зала")]
        public decimal TradingHallArea { get; set; }
        /// <summary>
        /// Среднее количество чеков в сутки
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Среднее количество чеков в сутки")]
        public int ChequePerDay { get; set; }
        /// <summary>
        /// Выручка в месяц
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Выручка в месяц")]
        public decimal RevenueAvg { get; set; }
        /// <summary>
        /// Общий тамбур с раковиной
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Общий тамбур с раковиной")]
        public bool HasJointRestroomEntrance { get; set; }
        /// <summary>
        /// Сибилла
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Сибилла")]
        public bool HasSibilla { get; set; }
        /// <summary>
        /// Выпечка
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Выпечка")]
        public bool HasBakery { get; set; }
        /// <summary>
        /// Торты
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Торты")]
        public bool HasCakes { get; set; }
        /// <summary>
        /// Фри
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Фри")]
        public bool HasFrenchFry { get; set; }
        /// <summary>
        /// Мармит
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Мармит")]
        public bool HasMarmite { get; set; }
        /// <summary>
        /// Кухня
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Кухня")]
        public bool HasKitchen { get; set; }
        /// <summary>
        /// Количество кофемашин
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество кофемашин")]
        public int CoffeeMachineTotal { get; set; }
        /// <summary>
        /// Количество посудомоечных машин
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Количество посудомоечных машин")]
        public int DishWashingMachineTotal { get; set; }
    }
}
