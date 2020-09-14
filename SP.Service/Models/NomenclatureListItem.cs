namespace SP.Service.Models
{
    public class NomenclatureListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код Петроникса
        /// </summary>
        public string PetronicsCode { get; set; }
        /// <summary>
        /// Наименование Петроникса
        /// </summary>
        public string PetronicsName { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// ID группы номенклатуры
        /// </summary>
        public int NomenclatureGroupId { get; set; }
        /// <summary>
        /// Группа номенклатуры
        /// </summary>
        public string NomenclatureGroupName { get; set; }
        /// <summary>
        /// Срок полезного использования, месяцев
        /// </summary>
        public int UsefulLife { get; set; }
        /// <summary>
        /// В работе
        /// </summary>
        public string Active { get; set; }
    }
}
