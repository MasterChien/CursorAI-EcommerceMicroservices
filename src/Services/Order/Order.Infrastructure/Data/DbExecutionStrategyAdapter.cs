using Microsoft.EntityFrameworkCore.Storage;
using Order.Core.Interfaces;

namespace Order.Infrastructure.Data
{
    /// <summary>
    /// Lớp adapter cho IExecutionStrategy của EntityFrameworkCore
    /// </summary>
    public class DbExecutionStrategyAdapter : IDbExecutionStrategy
    {
        private readonly IExecutionStrategy _executionStrategy;

        public DbExecutionStrategyAdapter(IExecutionStrategy executionStrategy)
        {
            _executionStrategy = executionStrategy ?? throw new ArgumentNullException(nameof(executionStrategy));
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return await _executionStrategy.ExecuteAsync(operation);
        }
    }
} 