using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SP.Core.Model;

namespace SP.Service.Models
{
    public class UserModel
    {
        // from Person

        /// <summary>
        /// Идентификатор персоны
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        [DisplayName("Отчество")]
        public string MiddleName { get; set; }

        // from IdentityUser<string>

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        [DisplayName("Логин")]
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        public string Email { get; set; }

        // from ApplicationUser

        /// <summary>
        /// Доступ заблокирован
        /// </summary>
        [DisplayName("Доступ заблокирован")]
        public bool Inactive { get; set; }
        /// <summary>
        /// Дата регистрации
        /// </summary>
        [DisplayName("Дата регистрации")]
        public DateTime RegistrationDate { get; set; }

        // from IdenityUserRole<string>

        /// <summary>
        /// Роль пользователя (AspNetRole)
        /// </summary>
        [Required]
        [DisplayName("Роль пользователя")]
        public string RoleId { get; set; }
    }
}
