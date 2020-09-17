using System;

namespace SP.Service.Models
{
    public class CalcSheetReportListItem
    {
        public string NomenclatureGroupName { get; set; }
        public string NomenclatureName { get; set; }
        public string StationNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Plan { get; set; }
        public decimal Quantity { get; set; }
    }
}
