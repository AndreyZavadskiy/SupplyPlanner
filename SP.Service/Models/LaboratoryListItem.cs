using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SP.Service.Models
{
    /// <summary>
    /// Нефтебаза
    /// </summary>
    public class LaboratoryListItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название объекта сети
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Количество персонала
        /// </summary>
        public int? PersonnelTotal { get; set; }
        /// <summary>
        /// Количество закрепленных АЗС
        /// </summary>
        public int? ServicingGasStationTotal { get; set; }
        /// <summary>
        /// Среднее количество проб в месяц
        /// </summary>
        public int? AverageTestPerMonth { get; set; }
        /// <summary>
        /// Количество рабочих помещений
        /// </summary>
        public int? WorkingRoomTotal { get; set; }
        /// <summary>
        /// Количество комнат приема пищи
        /// </summary>
        public int? DiningRoomTotal { get; set; }
        /// <summary>
        /// Количество санузлов
        /// </summary>
        public int? RestroomTotal { get; set; }
        /// <summary>
        /// Наличие скважины
        /// </summary>
        public bool HasWell { get; set; }
        /// <summary>
        /// Наличие анализатора cпектроскана S
        /// </summary>
        public bool HasSpectroscan { get; set; }
        /// <summary>
        /// Наличие анализатора Sindy
        /// </summary>
        public bool HasSindyAnalyzer { get; set; }
        /// <summary>
        /// Количество печатей/штемпелей
        /// </summary>
        public int? StampTotal { get; set; }
    }
}
