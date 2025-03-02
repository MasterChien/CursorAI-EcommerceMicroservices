namespace Order.Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .GreaterThan(0).WithMessage("{PropertyName} phải lớn hơn 0.");

        RuleFor(p => p.CustomerId)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .MaximumLength(50).WithMessage("{PropertyName} không được vượt quá 50 ký tự.");

        RuleFor(p => p.CustomerName)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá 100 ký tự.");

        RuleFor(p => p.CustomerEmail)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .EmailAddress().WithMessage("{PropertyName} không đúng định dạng email.")
            .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá 100 ký tự.");

        RuleFor(p => p.ShippingAddress)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .MaximumLength(200).WithMessage("{PropertyName} không được vượt quá 200 ký tự.");

        RuleFor(p => p.BillingAddress)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .MaximumLength(200).WithMessage("{PropertyName} không được vượt quá 200 ký tự.");

        RuleFor(p => p.PaymentMethod)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .Must(BeValidPaymentMethod).WithMessage("{PropertyName} không hợp lệ. Các giá trị hợp lệ: CreditCard, BankTransfer, Cash, Crypto, Other.");

        RuleFor(p => p.Status)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .Must(BeValidOrderStatus).WithMessage("{PropertyName} không hợp lệ. Các giá trị hợp lệ: Pending, Processing, Shipped, Delivered, Cancelled, Refunded.");

        RuleFor(p => p.OrderItems)
            .NotEmpty().WithMessage("Đơn hàng phải có ít nhất một mục.")
            .Must(items => items.Count > 0).WithMessage("Đơn hàng phải có ít nhất một mục.");

        RuleForEach(p => p.OrderItems).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(50).WithMessage("{PropertyName} không được vượt quá 50 ký tự.");

            item.RuleFor(i => i.ProductName)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá 100 ký tự.");

            item.RuleFor(i => i.ProductSku)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(50).WithMessage("{PropertyName} không được vượt quá 50 ký tự.");

            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} phải lớn hơn hoặc bằng 0.");

            item.RuleFor(i => i.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} phải lớn hơn hoặc bằng 0.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("{PropertyName} phải lớn hơn 0.");
        });
    }

    private bool BeValidPaymentMethod(string paymentMethod)
    {
        return Enum.TryParse<PaymentMethod>(paymentMethod, true, out _);
    }

    private bool BeValidOrderStatus(string status)
    {
        return Enum.TryParse<OrderStatus>(status, true, out _);
    }
} 