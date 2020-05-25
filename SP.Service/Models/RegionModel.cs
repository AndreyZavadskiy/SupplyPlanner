using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Регион
    /// </summary>
    public class RegionModel
    {
        /// <summary>
        /// ID региона/территории
        /// </summary>
        [DisplayName("ID")]
        public int Id { get; set; }
        /// <summary>
        /// ID вышестоящего элемента (регион)
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Required(ErrorMessage = "Поле Наименование является обязательным.")]
        [DisplayName("Наименование")]
        public string Name { get; set; }
        /// <summary>
        /// Запись исключена
        /// </summary>
        [DisplayName("Запись исключена")]
        public bool Inactive { get; set; }
    }
}
