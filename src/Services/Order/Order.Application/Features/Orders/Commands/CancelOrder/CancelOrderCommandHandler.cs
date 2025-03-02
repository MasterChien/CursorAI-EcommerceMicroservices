namespace Order.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling order with ID {OrderId}", request.Id);

        // Lấy đơn hàng cần hủy
        var orderToCancel = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
        if (orderToCancel == null)
        {
            _logger.LogWarning("Đơn hàng với ID {OrderId} không tồn tại", request.Id);
            return false;
        }

        // Kiểm tra xem đơn hàng có thể hủy không
        if (orderToCancel.Status == OrderStatus.Shipped ||
            orderToCancel.Status == OrderStatus.Delivered)
        {
            _logger.LogWarning("Không thể hủy đơn hàng {OrderId} với trạng thái {Status}",
                               request.Id, orderToCancel.Status);
            return false;
        }

        try
        {
            // Sử dụng OrderStateMachine để chuyển trạng thái
            var result = await _unitOfWork.OrderStateMachine.TransitionToAsync(
                orderToCancel, 
                OrderStatus.Cancelled, 
                request.CancellationReason,
                "System");
                
            if (!result)
            {
                _logger.LogWarning("Không thể chuyển đơn hàng {OrderId} sang trạng thái Cancelled", request.Id);
                return false;
            }

            // Cập nhật ghi chú đơn hàng
            orderToCancel.Notes = !string.IsNullOrEmpty(orderToCancel.Notes)
                ? $"{orderToCancel.Notes}\nHủy đơn hàng: {request.CancellationReason}"
                : $"Hủy đơn hàng: {request.CancellationReason}";
            orderToCancel.LastModifiedDate = DateTime.UtcNow;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.OrderRepository.UpdateAsync(orderToCancel);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Đơn hàng {OrderId} đã được hủy thành công", request.Id);

            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Lỗi khi hủy đơn hàng {OrderId}", request.Id);
            throw;
        }
    }
} 