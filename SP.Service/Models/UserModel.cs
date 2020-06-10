using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class UserModel
    {
        // from Person

        /// <summary>
        /// Идентификатор персоны
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код персоны
        /// </summary>
        [DisplayName("Код")]
        public string Code { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required(ErrorMessage = "Поле Фамилия является обязательным.")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [Required(ErrorMessage = "Поле Имя является обязательным.")]
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
        [Required(ErrorMessage = "Поле Логин является обязательным.")]
        [DisplayName("Логин")]
        public string UserName { get; set; }
        /// <summary>
        /// Пароль при создании нового пользователя
        /// </summary>
        [DisplayName("Пароль")]
        [PasswordPropertyText]
        public string Password { get; set; }
        /// <summary>
        /// Повтор пароля
        /// </summary>
        [DisplayName("Повтор пароля")]
        [PasswordPropertyText]
        public string PasswordRepeat { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Поле Email является обязательным.")]
        [EmailAddress]
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
        [DataType(DataType.Date)]
        [DisplayName("Дата регистрации")]
        public DateTime RegistrationDate { get; set; }

        // from IdenityUserRole<string>

        /// <summary>
        /// Роль пользователя (AspNetRole)
        /// </summary>
        [Required(ErrorMessage = "Поле Роль является обязательным.")]
        [DisplayName("Роль пользователя")]
        public string RoleName { get; set; }
    }
}
