namespace Order.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "CreditCard";
    public string? PaymentTransactionId { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pending";
    public List<UpdateOrderItemDto> OrderItems { get; set; } = new List<UpdateOrderItemDto>();
}

public class UpdateOrderItemDto
{
    public int? Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Quantity { get; set; }
} 