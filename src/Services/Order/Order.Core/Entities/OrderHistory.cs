namespace Order.Core.Entities
{
    public class OrderHistory : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        
        public OrderStatus PreviousStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? Comment { get; set; }
        public string? UpdatedBy { get; set; }
    }
} 