namespace Order.Application.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductSku { get; set; }
        public string? PictureUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 