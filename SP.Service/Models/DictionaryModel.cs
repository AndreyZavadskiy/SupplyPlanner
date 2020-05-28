using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SP.Service.Models
{
    public class DictionaryModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [DisplayName("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Required(ErrorMessage = "Поле Name является обязательным")]
        [DisplayName("Наименование")]
        public string Name { get; set; }
        /// <summary>
        /// Запись исключена
        /// </summary>
        [DisplayName("Запись заблокирована")]
        public bool Inactive { get; set; }
    }
}
