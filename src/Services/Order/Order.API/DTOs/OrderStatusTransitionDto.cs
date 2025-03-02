namespace Order.API.DTOs
{
    /// <summary>
    /// DTO cho chuyển đổi trạng thái đơn hàng
    /// </summary>
    public class OrderStatusTransitionDto
    {
        /// <summary>
        /// Trạng thái mới cần chuyển đến
        /// </summary>
        public string NewStatus { get; set; } = string.Empty;
        
        /// <summary>
        /// Lý do chuyển đổi trạng thái (tùy chọn)
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Người thực hiện chuyển đổi (tùy chọn)
        /// </summary>
        public string? ChangedBy { get; set; }
    }
} 