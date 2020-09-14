using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SP.Data
{
    public class ApplicationRole : IdentityRole
    {
        /// <summary>
        /// Название роли для интерфейса
        /// </summary>
        [Required]
        [StringLength(256)]
        public string FriendlyName { get; set; }
    }
}
