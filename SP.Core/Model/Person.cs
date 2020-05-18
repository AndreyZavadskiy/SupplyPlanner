using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SP.Core.Model
{
    /*
     * Таблица Person и AspNetUsers взаимосвязаны только на уровне БД,
     * т.к. Person относится к Domain model проекта, AspNetUsers - подсистеме аутентификации ASP.Net Core.
     * Для обеспечения целостности добавлена внешняя ссылка на таблице AspNetUsers.
     * Для ускорения обратного поиска добавлен индекс по полю AspNetUserId.
     * Реализовано путем ручной правки файла миграции 20200511165555_20200511_Person
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
    }
}
