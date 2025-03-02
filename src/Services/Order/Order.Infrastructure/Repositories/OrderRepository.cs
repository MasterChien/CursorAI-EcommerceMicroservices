using Microsoft.EntityFrameworkCore;
using Order.Core.Entities;
using Order.Core.Interfaces;
using Order.Infrastructure.Data;
using System.Data.Common;
using Dapper;

namespace Order.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Core.Entities.Order>, IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Core.Entities.Order>> GetOrdersByCustomerIdAsync(string customerId)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }

    public async Task<Core.Entities.Order?> GetOrderByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<Core.Entities.Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? comment = null, string? updatedBy = null)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        var previousStatus = order.Status;
        order.Status = newStatus;

        // Cập nhật các thuộc tính liên quan đến trạng thái
        switch (newStatus)
        {
            case OrderStatus.Paid:
                order.PaidDate = DateTime.UtcNow;
                break;
            case OrderStatus.Shipped:
                order.ShippedDate = DateTime.UtcNow;
                break;
            case OrderStatus.Delivered:
                order.DeliveredDate = DateTime.UtcNow;
                break;
            case OrderStatus.Cancelled:
                order.CancelledDate = DateTime.UtcNow;
                order.CancellationReason = comment;
                break;
        }

        // Thêm vào lịch sử đơn hàng
        _context.OrderHistories.Add(new OrderHistory
        {
            OrderId = orderId,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            Comment = comment,
            UpdatedBy = updatedBy,
            CreatedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
    {
        // Sử dụng Dapper để truy vấn hiệu quả
        var connection = _context.Database.GetDbConnection();
        await EnsureConnectionOpenAsync(connection);

        var sql = @"
            SELECT COALESCE(SUM(TotalAmount), 0) 
            FROM Orders 
            WHERE Status IN (@paid, @shipped, @delivered) 
            AND CreatedDate BETWEEN @startDate AND @endDate";

        return await connection.ExecuteScalarAsync<decimal>(sql, new
        {
            paid = OrderStatus.Paid,
            shipped = OrderStatus.Shipped,
            delivered = OrderStatus.Delivered,
            startDate,
            endDate
        });
    }

    public async Task<bool> OrderExistsAsync(string orderNumber)
    {
        return await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<OrderHistory>> GetOrderHistoryAsync(int orderId)
    {
        return await _context.OrderHistories
            .Where(oh => oh.OrderId == orderId)
            .OrderByDescending(oh => oh.CreatedDate)
            .ToListAsync();
    }

    public async Task<Core.Entities.Order?> GetOrderByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task AddOrderHistoryAsync(OrderHistory orderHistory)
    {
        await _context.OrderHistories.AddAsync(orderHistory);
    }

    /// <summary>
    /// Thêm lịch sử trạng thái đơn hàng
    /// </summary>
    /// <param name="statusHistory">Thông tin lịch sử trạng thái</param>
    public async Task AddOrderStatusHistoryAsync(OrderStatusHistory statusHistory)
    {
        await _context.OrderStatusHistories.AddAsync(statusHistory);
    }
    
    /// <summary>
    /// Lấy lịch sử trạng thái của đơn hàng
    /// </summary>
    /// <param name="orderId">ID đơn hàng</param>
    /// <returns>Danh sách lịch sử trạng thái</returns>
    public async Task<IEnumerable<OrderStatusHistory>> GetOrderStatusHistoryAsync(int orderId)
    {
        return await _context.OrderStatusHistories
            .Where(sh => sh.OrderId == orderId)
            .OrderByDescending(sh => sh.ChangedDate)
            .ToListAsync();
    }
    
    /// <summary>
    /// Lấy đơn hàng theo ID kèm theo lịch sử trạng thái
    /// </summary>
    /// <param name="id">ID đơn hàng</param>
    /// <returns>Đơn hàng với lịch sử trạng thái</returns>
    public async Task<Core.Entities.Order?> GetOrderWithStatusHistoryAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    private async Task EnsureConnectionOpenAsync(DbConnection connection)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
    }
} 