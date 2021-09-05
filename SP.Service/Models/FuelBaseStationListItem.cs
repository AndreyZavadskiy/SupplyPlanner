using System.ComponentModel;

namespace SP.Service.Models
{
    /// <summary>
    /// Нефтебаза
    /// </summary>
    public class FuelBaseStationListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название объекта сети
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Количество персонала
        /// </summary>
        public int? PersonnelTotal { get; set; }
        /// <summary>
        /// Количество смен в сутки
        /// </summary>
        public int? ShiftPerDay { get; set; }
        /// <summary>
        /// Количество персонала в смену
        /// </summary>
        public int? PersonnelPerShift { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        public decimal? PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество флагштоков
        /// </summary>
        public int? FlagpoleTotal { get; set; }
        /// <summary>
        /// План поставок ж/д транспортом
        /// </summary>
        public decimal? RailwayDeliveryPlanTotal { get; set; }
        /// <summary>
        /// Количество бензовозов в год
        /// </summary>
        public int? FuelTrackPerYear { get; set; }
        /// <summary>
        /// Количество жд цистерн в год
        /// </summary>
        public int? RailwayTankPerYear { get; set; }
        /// <summary>
        /// Количество резервуаров
        /// </summary>
        public int? ReservoirTotal { get; set; }
        /// <summary>
        /// Количество рабочих мест
        /// </summary>
        public int? WorkingPlaceTotal { get; set; }
        /// <summary>
        /// Количество санузлов
        /// </summary>
        public int? RestroomTotal { get; set; }
        /// <summary>
        /// Количество топлива (92) в год
        /// </summary>
        public decimal? Fuel92PerYear { get; set; }
        /// <summary>
        /// Количество топлива (95) в год
        /// </summary>
        public decimal? Fuel95PerYear { get; set; }
        /// <summary>
        /// Количество топлива (100) в год
        /// </summary>
        public decimal? Fuel100PerYear { get; set; }
        /// <summary>
        /// Количество топлива (ДТ) в год
        /// </summary>
        public decimal? DieselFuelPerYear { get; set; }
        /// <summary>
        /// Автоматизация нефтебазы
        /// </summary>
        public bool HasFuelBaseAutomation { get; set; }
        /// <summary>
        /// Площадь обработки противогололёдной смесью
        /// </summary>
        public decimal? AntiIcingSquare { get; set; }
        /// <summary>
        /// Количество обработок противогололёдной смесью в год
        /// </summary>
        public int? AntiIcingPerYear { get; set; }
        /// <summary>
        /// Количество комнат приема пищи
        /// </summary>
        public int? DiningRoomTotal { get; set; }
    }
}
