using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SP.Core.Model
{
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
        /// ID пользователя
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
