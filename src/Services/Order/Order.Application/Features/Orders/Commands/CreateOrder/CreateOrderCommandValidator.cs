using FluentValidation;

namespace Order.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(p => p.CustomerId)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(50).WithMessage("{PropertyName} không được vượt quá 50 ký tự.");

            RuleFor(p => p.CustomerName)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá 100 ký tự.");

            RuleFor(p => p.CustomerEmail)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .EmailAddress().WithMessage("{PropertyName} phải là một địa chỉ email hợp lệ.")
                .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá 100 ký tự.");

            RuleFor(p => p.ShippingAddress)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(500).WithMessage("{PropertyName} không được vượt quá 500 ký tự.");

            RuleFor(p => p.BillingAddress)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.")
                .MaximumLength(500).WithMessage("{PropertyName} không được vượt quá 500 ký tự.");

            RuleFor(p => p.PaymentMethod)
                .NotEmpty().WithMessage("{PropertyName} không được để trống.");

            RuleFor(p => p.OrderItems)
                .NotEmpty().WithMessage("Đơn hàng phải có ít nhất một mục.")
                .Must(items => items.Count > 0).WithMessage("Đơn hàng phải có ít nhất một mục.");

            RuleForEach(p => p.OrderItems).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("ProductId không được để trống.");
                
                item.RuleFor(i => i.ProductName)
                    .NotEmpty().WithMessage("ProductName không được để trống.");
                
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
                
                item.RuleFor(i => i.UnitPrice)
                    .GreaterThanOrEqualTo(0).WithMessage("Đơn giá không được âm.");
            });
        }
    }
} 