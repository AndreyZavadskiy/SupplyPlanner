using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace SP.Core.Model
{
    /*
     * Таблица Person и AspNetUsers взаимосвязаны только на уровне БД,
     * т.к. Person относится к Domain model проекта, AspNetUsers - подсистеме аутентификации ASP.Net Core.
     * Для обеспечения целостности добавлена внешняя ссылка к таблице AspNetUsers.
     */

    /// <summary>
    /// Персона
    /// </summary>
    [Table("Person")]
    public class Person
    {
        /// <summary>
        /// Идентификатор персоны
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код персоны
        /// </summary>
        [StringLength(20)]
        public string Code { get; set; }
        /// <summary>
        /// ID пользователя ASP.Net
        /// </summary>
        [Required]
        [StringLength(450)]
        public string AspNetUserId { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(50)]
        public string MiddleName { get; set; }

        #region Navigation properties

        public IEnumerable<PersonTerritory> PersonTerritories { get; set; }

        #endregion

        public static string ConcatenateFio(string lastName, string firstName, string middleName)
        {
            return lastName 
                   + (string.IsNullOrWhiteSpace(firstName) ? " " + firstName : string.Empty)
                   + (string.IsNullOrWhiteSpace(middleName) ? " " + middleName : string.Empty);
        }
    }
}
