using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Нефтебаза
    /// </summary>
    public class OfficeListItem
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
        /// Количество отделов
        /// </summary>
        public int? DepartmentTotal { get; set; }
        /// <summary>
        /// Количество флагштоков
        /// </summary>
        public int? FlagpoleTotal { get; set; }
        /// <summary>
        /// Централизованное водоснабжение
        /// </summary>
        public bool HasCentralWaterSupply { get; set; }
    }
}
