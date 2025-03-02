namespace Order.API.DTOs
{
    /// <summary>
    /// DTO cho lịch sử trạng thái đơn hàng
    /// </summary>
    public class OrderStatusHistoryDto
    {
        /// <summary>
        /// ID của bản ghi lịch sử
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID của đơn hàng
        /// </summary>
        public int OrderId { get; set; }
        
        /// <summary>
        /// Trạng thái trước đó
        /// </summary>
        public string PreviousStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// Trạng thái mới
        /// </summary>
        public string NewStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// Thời gian thay đổi trạng thái
        /// </summary>
        public DateTime ChangedDate { get; set; }
        
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