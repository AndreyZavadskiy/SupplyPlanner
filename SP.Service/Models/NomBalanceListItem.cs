using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Service.Models
{
    public class NomBalanceListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код по Номенклатуре
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование по Номенклатуре
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// АЗС
        /// </summary>
        public string GasStationName { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnitName { get; set; }
        /// <summary>
        /// Дата актуализации
        /// </summary>
        public DateTime LastUpdate { get; set; }

    }
}
