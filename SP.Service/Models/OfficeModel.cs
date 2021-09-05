using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Нефтебаза
    /// </summary>
    public class OfficeModel
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
        /// Количество персонала
        /// </summary>
        [DisplayName("Количество персонала")]
        public int? PersonnelTotal { get; set; }
        /// <summary>
        /// Количество отделов
        /// </summary>
        [DisplayName("Количество отделов")]
        public int? DepartmentTotal { get; set; }
        /// <summary>
        /// Количество флагштоков
        /// </summary>
        [DisplayName("Количество флагштоков")]
        public int? FlagpoleTotal { get; set; }
        /// <summary>
        /// Централизованное водоснабжение
        /// </summary>
        [DisplayName("Централизованное водоснабжение")]
        public bool HasCentralWaterSupply { get; set; }
    }
}
