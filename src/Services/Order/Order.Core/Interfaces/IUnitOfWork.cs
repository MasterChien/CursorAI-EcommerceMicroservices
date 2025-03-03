using Order.Core.StateMachine;

namespace Order.Core.Interfaces 
{ 
    public interface IUnitOfWork : IDisposable 
    { 
        /// <summary>
        /// Repository cho đơn hàng
        /// </summary>
        IOrderRepository OrderRepository { get; }
        
        /// <summary>
        /// State machine cho đơn hàng
        /// </summary>
        IOrderStateMachine OrderStateMachine { get; }
        
        /// <summary>
        /// Lưu thay đổi vào database
        /// </summary>
        Task<int> SaveChangesAsync(); 
        
        /// <summary>
        /// Bắt đầu transaction
        /// </summary>
        Task BeginTransactionAsync(); 
        
        /// <summary>
        /// Commit transaction
        /// </summary>
        Task CommitTransactionAsync(); 
        
        /// <summary>
        /// Rollback transaction
        /// </summary>
        Task RollbackTransactionAsync(); 
        
        /// <summary>
        /// Tạo execution strategy cho database
        /// </summary>
        /// <returns>Một đối tượng có thể thực hiện các giao dịch có thể thử lại</returns>
        IDbExecutionStrategy CreateExecutionStrategy();
    } 
}
