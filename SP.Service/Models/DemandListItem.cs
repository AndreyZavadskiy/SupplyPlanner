﻿using System;

namespace SP.Service.Models
{
    /// <summary>
    /// Расчетный лист для вывода остатков и потребности
    /// </summary>
    public class DemandListItem
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
        /// Фикс.количество
        /// </summary>
        public decimal? FixedAmount { get; set; }
        /// <summary>
        /// Формула расчета
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// Округление
        /// </summary>
        public string RoundingName { get; set; }
        /// <summary>
        /// План потребности
        /// </summary>
        public decimal Plan { get; set; }
        /// <summary>
        /// Количество для заказа
        /// </summary>
        public decimal OrderQuantity { get; set; }
        /// <summary>
        /// Количество последнего заказа
        /// </summary>
        public decimal LastQuantity { get; set; }
        /// <summary>
        /// Дата последнего заказа
        /// </summary>
        public DateTime? LastOrderDate { get; set; }
    }
}
