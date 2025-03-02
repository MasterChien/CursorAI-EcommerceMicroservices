using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Core.Entities;
using Order.Core.Interfaces;

namespace Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order for customer {CustomerId}", request.CustomerId);

        // Tạo đơn hàng mới
        var order = new Core.Entities.Order
        {
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            OrderNumber = GenerateOrderNumber(),
            Status = OrderStatus.Pending,
            ShippingAddress = request.ShippingAddress,
            BillingAddress = request.BillingAddress,
            PaymentMethod = ParsePaymentMethod(request.PaymentMethod),
            PaymentTransactionId = request.PaymentTransactionId,
            Notes = request.Notes,
            CreatedDate = DateTime.UtcNow
        };

        // Thêm các mục đơn hàng
        decimal totalAmount = 0;
        foreach (var item in request.OrderItems)
        {
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductSku = item.ProductSku,
                PictureUrl = item.PictureUrl,
                UnitPrice = item.UnitPrice,
                DiscountAmount = item.DiscountAmount,
                Quantity = item.Quantity
            };
            
            orderItem.CalculateTotalPrice();
            totalAmount += orderItem.TotalPrice;
            
            order.OrderItems.Add(orderItem);
        }

        order.TotalAmount = totalAmount;

        try
        {
            // Thêm đơn hàng và lưu thay đổi thông qua UnitOfWork
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Order {OrderId} created successfully", order.Id);
            return order.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
            throw;
        }
    }

    private string GenerateOrderNumber()
    {
        // Tạo mã đơn hàng: ORDyyyyMMddHHmmss
        return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    private PaymentMethod ParsePaymentMethod(string paymentMethod)
    {
        if (Enum.TryParse<PaymentMethod>(paymentMethod, true, out var result))
        {
            return result;
        }
        
        return PaymentMethod.Other;
    }
} 