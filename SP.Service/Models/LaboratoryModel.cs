using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Лаборатория
    /// </summary>
    public class LaboratoryModel
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
        /// Количество закрепленных АЗС
        /// </summary>
        [DisplayName("Количество закрепленных АЗС")]
        public int? ServicingGasStationTotal { get; set; }
        /// <summary>
        /// Среднее количество проб в месяц
        /// </summary>
        [DisplayName("Среднее количество проб в месяц")]
        public int? AverageTestPerMonth { get; set; }
        /// <summary>
        /// Количество рабочих помещений
        /// </summary>
        [DisplayName("Количество рабочих помещений")]
        public int? WorkingRoomTotal { get; set; }
        /// <summary>
        /// Количество комнат приема пищи
        /// </summary>
        [DisplayName("Количество комнат приема пищи")]
        public int? DiningRoomTotal { get; set; }
        /// <summary>
        /// Количество санузлов
        /// </summary>
        [DisplayName("Количество санузлов")]
        public int? RestroomTotal { get; set; }
        /// <summary>
        /// Наличие скважины
        /// </summary>
        [DisplayName("Наличие скважины")]
        public bool HasWell { get; set; }
        /// <summary>
        /// Наличие анализатора cпектроскана S
        /// </summary>
        [DisplayName("Наличие анализатора cпектроскана S")]
        public bool HasSpectroscan { get; set; }
        /// <summary>
        /// Наличие анализатора Sindy
        /// </summary>
        [DisplayName("Наличие анализатора Sindy")]
        public bool HasSindyAnalyzer { get; set; }
        /// <summary>
        /// Количество печатей/штемпелей
        /// </summary>
        [DisplayName("Количество печатей/штемпелей")]
        public int? StampTotal { get; set; }
        /// <summary>
        /// Количество специалистов, проводящих испытание электрооборудования на объекте (состав бригады)
        /// </summary>
        [DisplayName("Количество специалистов, проводящих испытание электрооборудования на объекте")]
        public int? SpecialistTotalForElectricalTest { get; set; }
        /// <summary>
        /// Количество объектов испытаний электрооборудования в год
        /// </summary>
        [DisplayName("Количество объектов испытаний электрооборудования в год")]
        public int? ElectricalTestPerYear { get; set; }
        /// <summary>
        /// Режим работы лаборатории
        /// </summary>
        [DisplayName("Режим работы лаборатории")]
        public string LaboratoryWorkSchedule { get; set; }
    }
}
