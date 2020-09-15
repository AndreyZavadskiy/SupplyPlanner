using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SP.Core.Model
{
    [Table("CalcSheetHistory")]
    public class CalcSheetHistory
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID оригинальной записи
        /// </summary>
        public int RecordId { get; set; }
        /// <summary>
        /// Дата вступления в силу
        /// </summary>
        public DateTime EffectiveDate { get; set; }
        /// <summary>
        /// ID Номенклатуры
        /// </summary>
        public int NomenclatureId { get; set; }
        /// <summary>
        /// ID АЗС
        /// </summary>
        public int GasStationId { get; set; }
        /// <summary>
        /// Остаток
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Фикс.количество
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal? FixedAmount { get; set; }
        /// <summary>
        /// Формула расчета
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// Кратность партии
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal MultipleFactor { get; set; }
        /// <summary>
        /// Округление
        /// 1 - вниз, 2 - вверх, 3 - до ближайшего целого
        /// </summary>
        public int Rounding { get; set; }
        /// <summary>
        /// План потребности
        /// </summary>
        [Column(TypeName = "decimal(19,4)")]
        public decimal Plan { get; set; }

        /// <summary>
        /// Дата актуализации
        /// </summary>
        public DateTime LastUpdate { get; set; }

        public static CalcSheetHistory CreateHistoryRecord(CalcSheet original, DateTime? effectiveDate = null)
        {
            var historyRecord = new CalcSheetHistory
            {
                RecordId = original.Id,
                EffectiveDate = effectiveDate ?? DateTime.Now,
                NomenclatureId = original.NomenclatureId,
                GasStationId = original.GasStationId,
                Quantity = original.Quantity,
                FixedAmount = original.FixedAmount,
                Formula = original.Formula,
                MultipleFactor = original.MultipleFactor,
                Rounding = original.Rounding,
                Plan = original.Plan,
                LastUpdate = original.LastUpdate
            };

            return historyRecord;
        }
    }
}
