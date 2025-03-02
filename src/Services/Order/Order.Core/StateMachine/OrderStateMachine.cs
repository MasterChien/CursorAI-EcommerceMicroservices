namespace Order.Core.StateMachine
{
    /// <summary>
    /// Triển khai State Machine cho đơn hàng
    /// </summary>
    public class OrderStateMachine : IOrderStateMachine
    {
        // Định nghĩa ma trận chuyển đổi trạng thái: key là trạng thái hiện tại, value là danh sách trạng thái có thể chuyển đến
        private readonly Dictionary<OrderStatus, List<OrderStatus>> _validTransitions = new()
        {
            { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
            { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Paid, OrderStatus.Cancelled } },
            { OrderStatus.Paid, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Refunded } },
            { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered, OrderStatus.Refunded } },
            { OrderStatus.Delivered, new List<OrderStatus> { OrderStatus.Refunded } },
            { OrderStatus.Cancelled, new List<OrderStatus>() },  // Không thể chuyển từ Cancelled sang trạng thái khác
            { OrderStatus.Refunded, new List<OrderStatus>() }    // Không thể chuyển từ Refunded sang trạng thái khác
        };

        /// <summary>
        /// Kiểm tra xem có thể chuyển từ trạng thái hiện tại sang trạng thái mới không
        /// </summary>
        /// <param name="currentState">Trạng thái hiện tại</param>
        /// <param name="newState">Trạng thái mới</param>
        /// <returns>True nếu có thể chuyển trạng thái, ngược lại False</returns>
        public bool CanTransitionTo(OrderStatus currentState, OrderStatus newState)
        {
            // Nếu trạng thái hiện tại và trạng thái mới giống nhau, không cần chuyển đổi
            if (currentState == newState)
                return true;

            // Kiểm tra xem trạng thái hiện tại có trong ma trận không
            if (!_validTransitions.ContainsKey(currentState))
                return false;

            // Kiểm tra xem trạng thái mới có trong danh sách trạng thái có thể chuyển đến từ trạng thái hiện tại không
            return _validTransitions[currentState].Contains(newState);
        }

        /// <summary>
        /// Lấy danh sách các trạng thái có thể chuyển đến từ trạng thái hiện tại
        /// </summary>
        /// <param name="currentState">Trạng thái hiện tại</param>
        /// <returns>Danh sách trạng thái có thể chuyển đến</returns>
        public IEnumerable<OrderStatus> GetPossibleTransitions(OrderStatus currentState)
        {
            if (_validTransitions.ContainsKey(currentState))
                return _validTransitions[currentState];
            
            return Enumerable.Empty<OrderStatus>();
        }

        /// <summary>
        /// Thực hiện chuyển đổi trạng thái đơn hàng
        /// </summary>
        /// <param name="order">Đơn hàng cần chuyển đổi trạng thái</param>
        /// <param name="newState">Trạng thái mới</param>
        /// <param name="reason">Lý do chuyển đổi (tùy chọn)</param>
        /// <param name="updatedBy">Người thực hiện chuyển đổi (tùy chọn)</param>
        /// <returns>True nếu chuyển đổi thành công, ngược lại False</returns>
        public Task<bool> TransitionToAsync(Entities.Order order, OrderStatus newState, string? reason = null, string? updatedBy = null)
        {
            // Nếu không thể chuyển đổi, trả về false
            if (!CanTransitionTo(order.Status, newState))
                return Task.FromResult(false);

            // Nếu trạng thái giống nhau, không cần cập nhật
            if (order.Status == newState)
                return Task.FromResult(true);

            // Tạo mới lịch sử trạng thái
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.Id,
                PreviousStatus = order.Status,
                NewStatus = newState,
                ChangedDate = DateTime.UtcNow,
                Reason = reason,
                ChangedBy = updatedBy
            };

            // Thêm vào lịch sử trạng thái của đơn hàng
            order.StatusHistory.Add(statusHistory);

            // Cập nhật các trường liên quan đến trạng thái
            var previousStatus = order.Status;
            order.Status = newState;

            // Cập nhật các trường liên quan dựa vào trạng thái mới
            switch (newState)
            {
                case OrderStatus.Paid:
                    order.PaidDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Shipped:
                    order.ShippedDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredDate = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledDate = DateTime.UtcNow;
                    order.CancellationReason = reason;
                    break;
                case OrderStatus.Refunded:
                    // Xử lý logic hoàn tiền nếu cần
                    break;
            }

            return Task.FromResult(true);
        }
    }
} 