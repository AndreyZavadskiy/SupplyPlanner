using System.ComponentModel;

namespace SP.Service.Models
{
    /// <summary>
    /// АЗС
    /// </summary>
    public class GasStationListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код КССС
        /// </summary>
        [DisplayName("Код КССС")]
        public int CodeKSSS { get; set; }
        /// <summary>
        /// Код SAP
        /// </summary>
        [DisplayName("Код SAP")]
        public string CodeSAP { get; set; }
        /// <summary>
        /// Номер АЗС
        /// </summary>
        [DisplayName("Номер АЗС")]
        public string StationNumber { get; set; }
        /// <summary>
        /// ID региона
        /// </summary>
        [DisplayName("ID региона")]
        public int RegionId { get; set; }
        /// <summary>
        /// Наименование региона
        /// </summary>
        [DisplayName("Наименование региона")]
        public string RegionName { get; set; }
        /// <summary>
        /// ID территории
        /// </summary>
        [DisplayName("ID территории")]
        public int TerritoryId { get; set; }
        /// <summary>
        /// Наименование территории
        /// </summary>
        [DisplayName("Наименование территории")]
        public string TerritoryName { get; set; }
        /// <summary>
        /// Населенный пункт
        /// </summary>
        [DisplayName("Населенный пункт")]
        public string SettlementName { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        [DisplayName("Адрес")]
        public string Address { get; set; }
        /// <summary>
        /// Месторасположение
        /// </summary>
        [DisplayName("Месторасположение")]
        public string StationLocationName { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        [DisplayName("Статус")]
        public string StationStatusName { get; set; }
        /// <summary>
        /// Сегмент
        /// </summary>
        [DisplayName("Сегмент")]
        public string SegmentName { get; set; }
        /// <summary>
        /// Кластер (уровень сервиса)
        /// </summary>
        [DisplayName("Кластер (уровень сервиса)")]
        public string ServiceLevelName { get; set; }
        /// <summary>
        /// Формат операторной
        /// </summary>
        [DisplayName("Формат операторной")]
        public string OperatorRoomFormatName { get; set; }
        /// <summary>
        /// Система управления
        /// </summary>
        [DisplayName("Система управления")]
        public string ManagementSystemName { get; set; }
        /// <summary>
        /// Режим работы торгового зала
        /// </summary>
        [DisplayName("Режим работы торгового зала")]
        public string TradingHallOperatingModeName { get; set; }
        /// <summary>
        /// Санузел для клиентов
        /// </summary>
        [DisplayName("Санузел для клиентов")]
        public string ClientRestroomName { get; set; }
        /// <summary>
        /// Расчетно-кассовый узел
        /// </summary>
        [DisplayName("Расчетно-кассовый узел")]
        public string CashboxLocationName { get; set; }
        /// <summary>
        /// Размер торгового зала
        /// </summary>
        [DisplayName("Размер торгового зала")]
        public string TradingHallSizeName { get; set; }
        /// <summary>
        /// Вид термоленты
        /// </summary>
        public string CashRegisterTapeName { get; set; }
        /// <summary>
        /// Количество касс
        /// </summary>
        [DisplayName("Количество касс")]
        public int CashboxTotal { get; set; }
        /// <summary>
        /// Количество АРМ менеджера
        /// </summary>
        [DisplayName("Количество АРМ менеджера")]
        public int ManagerArmTotal { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        [DisplayName("Количество персонала в сутки")]
        public decimal PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество ТРК
        /// </summary>
        [DisplayName("Количество ТРК")]
        public int FuelDispenserTotal { get; set; }
        /// <summary>
        /// Количество постов ТРК
        /// </summary>
        [DisplayName("Количество постов ТРК")]
        public int FuelDispenserPostTotal { get; set; }
        /// <summary>
        /// Количество постов ТРК
        /// </summary>
        [DisplayName("Количество постов ТРК без навеса")]
        public int FuelDispenserPostWithoutShedTotal { get; set; }
        /// <summary>
        /// Количество сан.комнат для клиентов
        /// </summary>
        [DisplayName("Количество сан.комнат для клиентов")]
        public int ClientRestroomTotal { get; set; }
        /// <summary>
        /// Количество тамбуров для клиентов
        /// </summary>
        [DisplayName("Количество тамбуров для клиентов")]
        public int ClientTambourTotal { get; set; }
        /// <summary>
        /// Количество раковин для клиентов
        /// </summary>
        [DisplayName("Количество раковин для клиентов")]
        public int ClientSinkTotal { get; set; }
        /// <summary>
        /// Площадь торгового зала
        /// </summary>
        [DisplayName("Площадь торгового зала")]
        public decimal? TradingHallArea { get; set; }
        /// <summary>
        /// Среднее количество чеков в сутки
        /// </summary>
        [DisplayName("Среднее количество чеков в сутки")]
        public decimal ChequePerDay { get; set; }
        /// <summary>
        /// Выручка в месяц
        /// </summary>
        [DisplayName("Выручка в месяц")]
        public decimal RevenueAvg { get; set; }
        /// <summary>
        /// Сибилла
        /// </summary>
        [DisplayName("Сибилла")]
        public bool HasSibilla { get; set; }
        /// <summary>
        /// Выпечка
        /// </summary>
        [DisplayName("Выпечка")]
        public bool HasBakery { get; set; }
        /// <summary>
        /// Торты
        /// </summary>
        [DisplayName("Торты")]
        public bool HasCakes { get; set; }
        /// <summary>
        /// Количество фритюрных аппаратов
        /// </summary>
        [DisplayName("Количество фритюрных аппаратов")]
        public int DeepFryTotal { get; set; }
        /// <summary>
        /// Мармит
        /// </summary>
        [DisplayName("Мармит")]
        public bool HasMarmite { get; set; }
        /// <summary>
        /// Кухня
        /// </summary>
        [DisplayName("Кухня")]
        public bool HasKitchen { get; set; }
        /// <summary>
        /// Количество кофемашин
        /// </summary>
        [DisplayName("Количество кофемашин")]
        public int CoffeeMachineTotal { get; set; }
        /// <summary>
        /// Количество посудомоечных машин
        /// </summary>
        [DisplayName("Количество посудомоечных машин")]
        public int DishWashingMachineTotal { get; set; }
        /// <summary>
        /// Имиджевый коэффициент
        /// </summary>
        [DisplayName("Имиджевый коэффициент")]
        public decimal RepresentativenessFactor { get; set; }
        /// <summary>
        /// Имиджевый коэффициент 3 квартал
        /// </summary>
        [DisplayName("Имиджевый коэффициент 3 кв.")]
        public decimal RepresentativenessFactor3Quarter { get; set; }
        /// <summary>
        /// Количество печей Меришеф
        /// </summary>
        [DisplayName("Количество печей Меришеф")]
        public int MerrychefTotal { get; set; }
        /// <summary>
        /// Уборка в день
        /// </summary>
        [DisplayName("Уборка в день")]
        public int DayCleaningTotal { get; set; }
        /// <summary>
        /// Уборка в ночь
        /// </summary>
        [DisplayName("Уборка в ночь")]
        public int NightCleaningTotal { get; set; }
        /// <summary>
        /// Расстановка заправки в день
        /// </summary>
        [DisplayName("Расстановка заправки в день")]
        public int DayRefuelingTotal { get; set; }
        /// <summary>
        /// Расстановка заправки в ночь
        /// </summary>
        [DisplayName("Расстановка заправки в ночь")]
        public int NightRefuelingTotal { get; set; }
        /// <summary>
        /// Участвует в проекте выдачи топливных карт
        /// </summary>
        [DisplayName("Топливные карты")]
        public bool HasFuelCardProgram { get; set; }
    }
}
