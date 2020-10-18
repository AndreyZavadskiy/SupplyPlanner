using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace SP.Web.ViewModels
{
    /// <summary>
    /// Загрузка остатков ТМЦ
    /// </summary>
    [Serializable]
    public class UploadInventoryViewModel
    {
        /// <summary>
        /// Дата обработки
        /// </summary>
        [Required(ErrorMessage = "Поле Дата загрузки обязательно для заполнения.")]
        [DisplayName("Дата загрузки")]
        public DateTime ProcessingDate { get; set; }
        /// <summary>
        /// Файлы для обработки
        /// </summary>
        [Required(ErrorMessage = "Выберите хотя бы один файл.")]
        [DisplayName("Файл(-ы)")]
        [IgnoreDataMember]
        public IFormFileCollection Files { get; set; }
        /// <summary>
        /// Лог обработки
        /// </summary>
        [DisplayName("Журнал выполнения")]
        public string ProcessingLog { get; set; }
    }
}
