namespace SP.Service.Models
{
    /// <summary>
    /// Элемент списка территорий
    /// </summary>
    public class TerritoryListItem
    {
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// Наименование региона
        /// </summary>
        public string RegionName { get; set; }
        /// <summary>
        /// Идентификатор территории
        /// </summary>
        public int TerritoryId { get; set; }
        /// <summary>
        /// Наименование территории
        /// </summary>
        public string TerritoryName { get; set; }
        /// <summary>
        /// Является активным элементом
        /// "1" - активный, "0" - заблокирован
        /// </summary>
        public string Active { get; set; }
    }
}
