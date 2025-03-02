namespace Order.Core.StateMachine;

/// <summary>
/// Interface cho state machine quản lý trạng thái đơn hàng
/// </summary>
public interface IOrderStateMachine
{
    /// <summary>
    /// Kiểm tra xem trạng thái hiện tại có thể chuyển sang trạng thái mới không
    /// </summary>
    /// <param name="currentState">Trạng thái hiện tại</param>
    /// <param name="newState">Trạng thái mới</param>
    /// <returns>True nếu có thể chuyển, ngược lại False</returns>
    bool CanTransitionTo(OrderStatus currentState, OrderStatus newState);

    /// <summary>
    /// Lấy danh sách các trạng thái tiếp theo có thể chuyển từ trạng thái hiện tại
    /// </summary>
    /// <param name="currentState">Trạng thái hiện tại</param>
    /// <returns>Danh sách các trạng thái có thể chuyển tiếp</returns>
    IEnumerable<OrderStatus> GetPossibleTransitions(OrderStatus currentState);

    /// <summary>
    /// Thực hiện chuyển đổi trạng thái đơn hàng
    /// </summary>
    /// <param name="order">Đơn hàng cần chuyển đổi</param>
    /// <param name="newState">Trạng thái mới</param>
    /// <param name="reason">Lý do chuyển đổi</param>
    /// <param name="updatedBy">Người thực hiện</param>
    /// <returns>True nếu chuyển đổi thành công, ngược lại False</returns>
    Task<bool> TransitionToAsync(Entities.Order order, OrderStatus newState, string? reason = null, string? updatedBy = null);
}