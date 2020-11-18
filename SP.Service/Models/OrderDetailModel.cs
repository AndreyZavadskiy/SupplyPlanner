namespace SP.Service.Models
{
    public class OrderDetailModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string GasStationName { get; set; }
        public string MeasureUnitName { get; set; }
        public int UsefulLife { get; set; }
        public decimal Quantity { get; set; }
    }
}
