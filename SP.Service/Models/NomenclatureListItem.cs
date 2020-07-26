using System;
using System.Collections.Generic;
using System.Text;

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
        /// Группа номенклатуры
        /// </summary>
        public string NomenclatureGroupName { get; set; }
        /// <summary>
        /// Срок полезного использования, месяцев
        /// </summary>
        public int UsefulLife { get; set; }
    }
}
