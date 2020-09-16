using System;

namespace SP.Service.Models
{
    public class ActionListItem
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }
    }
}
