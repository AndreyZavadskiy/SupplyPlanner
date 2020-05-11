using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SP.Data
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Является активным пользователем (доступ разрешен)
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата регистрации
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime RegistrationDate { get; set; }
    }
}
