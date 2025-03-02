using Order.Core.Entities;

namespace Order.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Entities.Order>
    {
        Task<IEnumerable<Entities.Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<Entities.Order?> GetOrderByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<Entities.Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? comment = null, string? updatedBy = null);
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
        Task<bool> OrderExistsAsync(string orderNumber);
        Task<IEnumerable<OrderHistory>> GetOrderHistoryAsync(int orderId);
        Task<Entities.Order?> GetOrderByIdWithItemsAsync(int id);
        Task AddOrderHistoryAsync(OrderHistory orderHistory);
        
        /// <summary>
        /// Thêm lịch sử trạng thái đơn hàng
        /// </summary>
        /// <param name="statusHistory">Thông tin lịch sử trạng thái</param>
        Task AddOrderStatusHistoryAsync(Entities.OrderStatusHistory statusHistory);
        
        /// <summary>
        /// Lấy lịch sử trạng thái của đơn hàng
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <returns>Danh sách lịch sử trạng thái</returns>
        Task<IEnumerable<Entities.OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId);
        
        /// <summary>
        /// Lấy đơn hàng theo ID kèm theo lịch sử trạng thái
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <returns>Đơn hàng với lịch sử trạng thái</returns>
        Task<Entities.Order?> GetOrderWithStatusHistoryAsync(int id);
    }
} 