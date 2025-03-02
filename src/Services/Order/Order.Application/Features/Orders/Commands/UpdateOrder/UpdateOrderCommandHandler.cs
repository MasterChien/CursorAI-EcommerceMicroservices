namespace Order.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrderCommandHandler> _logger;

    public UpdateOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order {OrderId} for customer {CustomerId}", request.Id, request.CustomerId);

        // Lấy đơn hàng cần cập nhật
        var orderToUpdate = await _unitOfWork.OrderRepository.GetOrderByIdWithItemsAsync(request.Id);
        if (orderToUpdate == null)
        {
            _logger.LogWarning("Đơn hàng với ID {OrderId} không tồn tại", request.Id);
            return false;
        }

        try
        {
            // Cập nhật thông tin cơ bản của đơn hàng
            orderToUpdate.CustomerName = request.CustomerName;
            orderToUpdate.CustomerEmail = request.CustomerEmail;
            orderToUpdate.ShippingAddress = request.ShippingAddress;
            orderToUpdate.BillingAddress = request.BillingAddress;
            orderToUpdate.PaymentMethod = ParsePaymentMethod(request.PaymentMethod);
            orderToUpdate.PaymentTransactionId = request.PaymentTransactionId;
            orderToUpdate.Notes = request.Notes;
            
            // Kiểm tra nếu trạng thái thay đổi, sử dụng state machine
            var newStatus = ParseOrderStatus(request.Status);
            if (orderToUpdate.Status != newStatus)
            {
                var result = await _unitOfWork.OrderStateMachine.TransitionToAsync(
                    orderToUpdate, 
                    newStatus, 
                    $"Cập nhật từ API: {request.Notes}",
                    "System");
                    
                if (!result)
                {
                    _logger.LogWarning("Không thể chuyển đơn hàng {OrderId} sang trạng thái {NewStatus}", 
                        request.Id, newStatus);
                    return false;
                }
            }
            
            orderToUpdate.LastModifiedDate = DateTime.UtcNow;

            // Cập nhật danh sách mục đơn hàng
            // 1. Xác định các mục cần thêm mới, cập nhật, hoặc xóa
            var existingItemIds = orderToUpdate.OrderItems.Select(i => i.Id).ToList();
            var requestItemIds = request.OrderItems.Where(i => i.Id.HasValue).Select(i => i.Id.Value).ToList();

            // Mục cần xóa: có trong existingItemIds nhưng không có trong requestItemIds
            var itemsToRemove = orderToUpdate.OrderItems.Where(i => !requestItemIds.Contains(i.Id)).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                orderToUpdate.OrderItems.Remove(itemToRemove);
            }

            decimal totalAmount = 0;

            // Cập nhật các mục hiện có và thêm mới các mục
            foreach (var itemDto in request.OrderItems)
            {
                if (itemDto.Id.HasValue && itemDto.Id.Value > 0)
                {
                    // Cập nhật mục hiện có
                    var existingItem = orderToUpdate.OrderItems.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.ProductName = itemDto.ProductName;
                        existingItem.ProductSku = itemDto.ProductSku;
                        existingItem.PictureUrl = itemDto.PictureUrl;
                        existingItem.UnitPrice = itemDto.UnitPrice;
                        existingItem.DiscountAmount = itemDto.DiscountAmount;
                        existingItem.Quantity = itemDto.Quantity;
                        existingItem.CalculateTotalPrice();
                        totalAmount += existingItem.TotalPrice;
                    }
                }
                else
                {
                    // Thêm mục mới
                    var newItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        ProductSku = itemDto.ProductSku,
                        PictureUrl = itemDto.PictureUrl,
                        UnitPrice = itemDto.UnitPrice,
                        DiscountAmount = itemDto.DiscountAmount,
                        Quantity = itemDto.Quantity
                    };
                    newItem.CalculateTotalPrice();
                    totalAmount += newItem.TotalPrice;
                    orderToUpdate.OrderItems.Add(newItem);
                }
            }

            // Cập nhật tổng giá trị đơn hàng
            orderToUpdate.TotalAmount = totalAmount;

            // Cập nhật đơn hàng trong database
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.OrderRepository.UpdateAsync(orderToUpdate);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderId} updated successfully", orderToUpdate.Id);
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating order {OrderId} for customer {CustomerId}", request.Id, request.CustomerId);
            throw;
        }
    }

    private PaymentMethod ParsePaymentMethod(string paymentMethod)
    {
        if (Enum.TryParse<PaymentMethod>(paymentMethod, true, out var result))
        {
            return result;
        }

        return PaymentMethod.CreditCard;
    }

    private OrderStatus ParseOrderStatus(string status)
    {
        if (Enum.TryParse<OrderStatus>(status, true, out var result))
        {
            return result;
        }

        return OrderStatus.Pending;
    }
} 