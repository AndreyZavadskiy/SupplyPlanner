using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Регион/территория
    /// </summary>
    [Table("Region")]
    public class Region
    {
        /// <summary>
        /// Идентификатор региона или территории
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Является активным
        /// </summary>
        public bool IsActive { get; set; }
    }
}
