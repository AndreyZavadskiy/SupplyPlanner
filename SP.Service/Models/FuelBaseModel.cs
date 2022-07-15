using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Нефтебаза
    /// </summary>
    public class FuelBaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        public int Id { get; set; }
        /// <summary>
        /// Название объекта сети
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Название")]
        public string ObjectName { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        [DisplayName("Адрес")]
        public string Address { get; set; }
        /// <summary>
        /// Код SAP
        /// </summary>
        [Required(ErrorMessage = "Поле является обязательным")]
        [DisplayName("Код SAP")]
        public string CodeSAP { get; set; }
        /// <summary>
        /// Количество персонала
        /// </summary>
        [DisplayName("Количество персонала")]
        public int? PersonnelTotal { get; set; }
        /// <summary>
        /// Количество смен в сутки
        /// </summary>
        [DisplayName("Количество смен в сутки")]
        public int? ShiftPerDay { get; set; }
        /// <summary>
        /// Количество персонала в смену
        /// </summary>
        [DisplayName("Количество персонала в смену")]
        public int? PersonnelPerShift { get; set; }
        /// <summary>
        /// Количество персонала в сутки
        /// </summary>
        [DisplayName("Количество персонала в сутки")]
        public decimal? PersonnelPerDay { get; set; }
        /// <summary>
        /// Количество флагштоков
        /// </summary>
        [DisplayName("Количество флагштоков")]
        public int? FlagpoleTotal { get; set; }
        /// <summary>
        /// План поставок ж/д транспортом
        /// </summary>
        [DisplayName("План поставок ж/д транспортом")]
        public decimal? RailwayDeliveryPlanTotal { get; set; }
        /// <summary>
        /// Количество бензовозов в год
        /// </summary>
        [DisplayName("Количество бензовозов в год")]
        public int? FuelTrackPerYear { get; set; }
        /// <summary>
        /// Количество жд цистерн в год
        /// </summary>
        [DisplayName("Количество жд цистерн в год")]
        public int? RailwayTankPerYear { get; set; }
        /// <summary>
        /// Количество резервуаров
        /// </summary>
        [DisplayName("Количество резервуаров")]
        public int? ReservoirTotal { get; set; }
        /// <summary>
        /// Количество рабочих мест
        /// </summary>
        [DisplayName("Количество рабочих мест")]
        public int? WorkingPlaceTotal { get; set; }
        /// <summary>
        /// Количество санузлов
        /// </summary>
        [DisplayName("Количество санузлов")]
        public int? RestroomTotal { get; set; }
        /// <summary>
        /// Количество топлива (92) в год
        /// </summary>
        [DisplayName("Количество топлива (92) в год")]
        public decimal? Fuel92PerYear { get; set; }
        /// <summary>
        /// Количество топлива (95) в год
        /// </summary>
        [DisplayName("Количество топлива (95) в год")]
        public decimal? Fuel95PerYear { get; set; }
        /// <summary>
        /// Количество топлива (100) в год
        /// </summary>
        [DisplayName("Количество топлива (100) в год")]
        public decimal? Fuel100PerYear { get; set; }
        /// <summary>
        /// Количество топлива (ДТ) в год
        /// </summary>
        [DisplayName("Количество топлива (ДТ) в год")]
        public decimal? DieselFuelPerYear { get; set; }
        /// <summary>
        /// Автоматизация нефтебазы
        /// </summary>
        [DisplayName("Автоматизация")]
        public bool HasFuelBaseAutomation { get; set; }
        /// <summary>
        /// Площадь обработки противогололёдной смесью
        /// </summary>
        [DisplayName("Площадь обработки противогололёдной смесью")]
        public decimal? AntiIcingSquare { get; set; }
        /// <summary>
        /// Количество обработок противогололёдной смесью в год
        /// </summary>
        [DisplayName("Количество обработок противогололёдной смесью в год")]
        public int? AntiIcingPerYear { get; set; }
        /// <summary>
        /// Количество комнат приема пищи
        /// </summary>
        [DisplayName("Количество комнат приема пищи")]
        public int? DiningRoomTotal { get; set; }
    }
}
