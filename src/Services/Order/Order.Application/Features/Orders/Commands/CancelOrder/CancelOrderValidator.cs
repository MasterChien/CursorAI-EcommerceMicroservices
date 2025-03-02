namespace Order.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .GreaterThan(0).WithMessage("{PropertyName} phải lớn hơn 0.");

        RuleFor(p => p.CancellationReason)
            .MaximumLength(500).WithMessage("{PropertyName} không được vượt quá 500 ký tự.");
    }
} 