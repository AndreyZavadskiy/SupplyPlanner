using System.ComponentModel;

namespace SP.Service.Models
{
    public class NomenclatureModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        [DisplayName("Код")]
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        public string Name { get; set; }
        /// <summary>
        /// Код Петроникса
        /// </summary>
        [DisplayName("Код Петроникса")]
        public string PetronicsCode { get; set; }
        /// <summary>
        /// Наименование Петроникса
        /// </summary>
        [DisplayName("Наименование Петроникса")]
        public string PetronicsName { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        [DisplayName("Ед.изм.")]
        public int MeasureUnitId { get; set; }
        /// <summary>
        /// Группа номенклатуры
        /// </summary>
        [DisplayName("Группа номенклатуры")]
        public int NomenclatureGroupId { get; set; }
        /// <summary>
        /// Срок полезного использования, месяцев
        /// </summary>
        [DisplayName("СПИ")]
        public int UsefulLife { get; set; }
        /// <summary>
        /// Запись исключена
        /// </summary>
        [DisplayName("Запись исключена")]
        public bool Inactive { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [DisplayName("Характеристики")]
        public string Description { get; set; }
    }
}
