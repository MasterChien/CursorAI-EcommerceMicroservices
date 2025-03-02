namespace Order.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductSku { get; set; }
        public string? PictureUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        
        // Tính toán tổng giá dựa trên số lượng và giá đơn vị
        public void CalculateTotalPrice()
        {
            TotalPrice = (UnitPrice - DiscountAmount) * Quantity;
        }
    }
} 