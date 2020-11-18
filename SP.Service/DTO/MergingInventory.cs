namespace SP.Service.DTO
{
    /// <summary>
    /// Распарсенные данные о ТМЦ
    /// </summary>
    public class MergingInventory
    {
        /// <summary>
        /// ID ТМЦ
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Код ТМЦ
        /// </summary>
        public string InventoryCode { get; set; }
        /// <summary>
        /// Наименование ТМЦ
        /// </summary>
        public string InventoryName { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        /// <summary>
        /// ID единицы измерения
        /// </summary>
        public int MeasureUnitId { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int GasStationId { get; set; }
        /// <summary>
        /// Номер АЗС
        /// </summary>
        public string StationNumber { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// </summary>
        public int? NomenclatureId { get; set; }
        /// <summary>
        /// Код Номенклатуры
        /// </summary>
        public string NomenclatureCode { get; set; }
        /// <summary>
        /// Название Номенклатуры
        /// </summary>
        public string NomenclatureName { get; set; }
        /// <summary>
        /// Имеется соответствие с Номенклатурой, либо еще не установлено
        /// </summary>
        public string Active { get; set; }
    }
}