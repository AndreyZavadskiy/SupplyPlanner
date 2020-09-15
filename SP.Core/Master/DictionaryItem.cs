using System.ComponentModel.DataAnnotations;

namespace SP.Core.Master
{
    public abstract class DictionaryItem
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
