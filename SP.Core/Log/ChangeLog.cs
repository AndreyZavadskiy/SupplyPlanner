using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.Log
{
    [Table("Change", Schema = "log")]
    public class ChangeLog
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор персоны
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime ChangeDate { get; set; }
        /// <summary>
        /// Имя таблицы
        /// </summary>
        [StringLength(50)]
        public string EntityName { get; set; }
        /// <summary>
        /// Наименования действия
        /// </summary>
        [StringLength(50)]
        public string ActionName { get; set; }
        /// <summary>
        /// id записи таблицы
        /// </summary>
        public int RecordId { get; set; }
        /// <summary>
        /// Предыдущее значение
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// Новое значение
        /// </summary>
        public string NewValue { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        #endregion
    }
}
