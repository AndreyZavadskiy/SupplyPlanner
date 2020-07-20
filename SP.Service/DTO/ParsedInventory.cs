using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Service.DTO
{
    /// <summary>
    /// Распарсенные данные о ТМЦ
    /// </summary>
    public class ParsedInventory
    {
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int? GasStationId { get; set; }
        /// <summary>
        /// Код SAP АЗС
        /// </summary>
        public string StationCodeSAP { get; set; }
        /// <summary>
        /// Название АЗС в Петроникс
        /// </summary>
        public string StationPetronicsName { get; set; }
        /// <summary>
        /// Код ТМЦ
        /// </summary>
        public string InventoryCode { get; set; }
        /// <summary>
        /// Наименование ТМЦ
        /// </summary>
        public string InventoryName { get; set; }
        /// <summary>
        /// ID единицы измерения
        /// </summary>
        public int? MeasureUnitId { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
