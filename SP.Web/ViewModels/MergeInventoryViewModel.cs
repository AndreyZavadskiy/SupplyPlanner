using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace SP.Web.ViewModels
{
    /// <summary>
    /// Автоматическое объединение ТМЦ с Номенклатурой
    /// </summary>
    [Serializable]
    public class MergeInventoryViewModel
    {
        /// <summary>
        /// Дата обработки
        /// </summary>
        [Required(ErrorMessage = "Поле Дата обработки обязательно для заполнения.")]
        [DisplayName("Дата загрузки")]
        public DateTime ProcessingDate { get; set; }
        /// <summary>
        /// Лог обработки
        /// </summary>
        [DisplayName("Журнал выполнения")]
        public string ProcessingLog { get; set; }
    }
}
