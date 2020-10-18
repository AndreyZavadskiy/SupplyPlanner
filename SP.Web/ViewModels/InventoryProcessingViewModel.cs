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
    public class InventoryProcessingViewModel
    {
        /// <summary>
        /// Дата обработки
        /// </summary>
        [Required(ErrorMessage = "Поле Дата обработки обязательно для заполнения.")]
        [DisplayName("Дата обработки")]
        public DateTime ProcessingDate { get; set; }
        /// <summary>
        /// Лог обработки
        /// </summary>
        [DisplayName("Журнал выполнения")]
        public string ProcessingLog { get; set; }
    }
}
