using System;

namespace Order.Core.Entities
{
    /// <summary>
    /// Entity để lưu trữ lịch sử trạng thái đơn hàng
    /// </summary>
    public class OrderStatusHistory : BaseEntity
    {
        /// <summary>
        /// ID của đơn hàng
        /// </summary>
        public int OrderId { get; set; }
        
        /// <summary>
        /// Đơn hàng liên quan
        /// </summary>
        public Order Order { get; set; } = null!;
        
        /// <summary>
        /// Trạng thái trước đó
        /// </summary>
        public OrderStatus PreviousStatus { get; set; }
        
        /// <summary>
        /// Trạng thái mới
        /// </summary>
        public OrderStatus NewStatus { get; set; }
        
        /// <summary>
        /// Thời gian thay đổi trạng thái
        /// </summary>
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Lý do thay đổi trạng thái (tùy chọn)
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Người thay đổi trạng thái (tùy chọn)
        /// </summary>
        public string? ChangedBy { get; set; }
    }
} 