﻿using System;

namespace SP.Service.Models
{
    public class OrderDetailReportListItem
    {
        public string NomenclatureGroupName { get; set; }
        public string NomenclatureName { get; set; }
        public string StationNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public int OrderNumber { get; set; }
    }
}
