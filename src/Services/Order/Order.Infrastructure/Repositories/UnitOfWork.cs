using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Order.Core.Interfaces;
using Order.Core.StateMachine;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    public IOrderRepository OrderRepository { get; }
    public IOrderStateMachine OrderStateMachine { get; }

    public UnitOfWork(
        OrderDbContext context, 
        IOrderRepository orderRepository,
        IOrderStateMachine orderStateMachine)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        OrderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        OrderStateMachine = orderStateMachine ?? throw new ArgumentNullException(nameof(orderStateMachine));
    }

    public async Task<int> SaveChangesAsync()
    {
        // Sử dụng execution strategy để tự động retry khi cần
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            return await _context.SaveChangesAsync();
        });
    }

    public async Task BeginTransactionAsync()
    {
        // Sử dụng execution strategy khi bắt đầu transaction
        if (_transaction != null)
        {
            return;
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        });
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }

        try
        {
            // Sử dụng execution strategy khi commit transaction
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            });
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
        {
            return;
        }

        try
        {
            // Sử dụng execution strategy khi rollback transaction
            await _transaction.RollbackAsync();
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public IDbExecutionStrategy CreateExecutionStrategy()
    {
        return new DbExecutionStrategyAdapter(_context.Database.CreateExecutionStrategy());
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
} 