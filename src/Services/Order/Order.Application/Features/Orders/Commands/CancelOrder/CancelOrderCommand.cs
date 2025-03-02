namespace Order.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<bool>
{
    public int Id { get; private set; }
    public string? CancellationReason { get; private set; }

    public CancelOrderCommand(int id, string? cancellationReason = null)
    {
        Id = id;
        CancellationReason = cancellationReason;
    }
} 