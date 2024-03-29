﻿using System;

namespace SP.Service.Models
{
    /// <summary>
    /// Расчетный лист для вывода остатков по номенклатуре
    /// </summary>
    public class BalanceListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
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
