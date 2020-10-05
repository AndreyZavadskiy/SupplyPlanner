namespace SP.Service.DTO
{
    /// <summary>
    /// Количество заказа позиции
    /// </summary>
    public class OrderQuantity
    {
        /// <summary>
        /// ID позиции остатка Номенклатуры
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// План заказа
        /// </summary>
        public decimal Plan { get; set; }
        /// <summary>
        /// Количество
        /// </summary>
        public decimal Quantity { get; set; }
    }
}
