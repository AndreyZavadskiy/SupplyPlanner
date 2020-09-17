using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SP.Core.Model;

namespace SP.Core.Log
{
    [Table("Change", Schema = "log")]
    public class ChangeLog
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime ChangeDate { get; set; }
        [StringLength(50)]
        public string EntityName { get; set; }
        public int RecordId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }

        #endregion
    }
}
