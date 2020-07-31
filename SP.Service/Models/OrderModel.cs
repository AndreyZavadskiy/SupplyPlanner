using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace SP.Service.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string PersonName { get; set; }
    }
}
