﻿using System;

namespace SP.Service.Models
{
    public class OrderModel
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string PersonName { get; set; }
    }
}
