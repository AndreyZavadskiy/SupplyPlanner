using System;

namespace SP.Service.Models
{
    public class ChangeListItem
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime ChangeDate { get; set; }
        public string EntityName { get; set; }
        public long RecordId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
