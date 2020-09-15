using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SP.Core.Model;

namespace SP.Core.History
{
    [Table("Action", Schema = "log")]
    public class ActionLog
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
        /// Дата действия
        /// </summary>
        public DateTime ActionDate { get; set; }
        /// <summary>
        /// Категория действий
        /// </summary>
        [StringLength(50)]
        public string Category { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        #endregion
    }
}
