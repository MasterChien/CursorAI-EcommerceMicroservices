using System.Collections.Generic;

namespace Order.Core.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentTransactionId { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancellationReason { get; set; }
        public string? Notes { get; set; }
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        /// <summary>
        /// Lịch sử trạng thái đơn hàng
        /// </summary>
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Paid = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Refunded = 6
    }
    
    public enum PaymentMethod
    {
        CreditCard = 0,
        BankTransfer = 1,
        Cash = 2,
        DigitalWallet = 3,
        Other = 4
    }
} 