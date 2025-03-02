namespace Order.Core.Interfaces
{
    /// <summary>
    /// Interface cho chiến lược thực thi database
    /// </summary>
    public interface IDbExecutionStrategy
    {
        /// <summary>
        /// Thực thi một hoạt động có thể thử lại
        /// </summary>
        /// <typeparam name="TResult">Kiểu của kết quả trả về</typeparam>
        /// <param name="operation">Hoạt động cần thực thi</param>
        /// <returns>Kết quả của hoạt động</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation);
    }
} 