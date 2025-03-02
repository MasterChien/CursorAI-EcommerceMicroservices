using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.DTOs;
using Order.Application.Features.Orders.Commands.CreateOrder;
using Order.Application.Features.Orders.Queries.GetOrderById;
using Order.Application.Features.Orders.Queries.GetOrdersList;
using Order.Application.Features.Orders.Commands.UpdateOrder;
using Order.Application.Features.Orders.Commands.CancelOrder;
using System.Net;
using Order.Core.Interfaces;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger, IUnitOfWork unitOfWork)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
        [FromQuery] string? customerId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrdersListQuery
        {
            CustomerId = customerId,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpPost]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, orderId);
    }

    [HttpGet("{id:int}", Name = "GetOrderById")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        _logger.LogInformation("Bắt đầu xử lý GetOrderById với ID = {OrderId}", id);
        
        var query = new GetOrderByIdQuery(id);
        _logger.LogInformation("Đã tạo GetOrderByIdQuery với ID = {OrderId}", id);
        
        var order = await _mediator.Send(query);
        _logger.LogInformation("Kết quả truy vấn order: {OrderResult}", order != null ? "Tìm thấy" : "Không tìm thấy");
        
        if (order == null)
        {
            _logger.LogWarning("Không tìm thấy đơn hàng với ID = {OrderId}", id);
            return NotFound();
        }
            
        _logger.LogInformation("Trả về đơn hàng với ID = {OrderId}", id);
        return Ok(order);
    }

    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerId(string customerId)
    {
        var query = new GetOrdersListQuery { CustomerId = customerId };
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(string status)
    {
        var query = new GetOrdersListQuery { Status = status };
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpGet("test/{id:int}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> TestGetOrderById(int id)
    {
        _logger.LogInformation("Test endpoint được gọi với ID = {OrderId}", id);
        return Ok($"Test endpoint được gọi với ID = {id}");
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<bool>> UpdateOrder(int id, [FromBody] UpdateOrderCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID trong URL không khớp với ID trong dữ liệu.");
        }

        _logger.LogInformation("Bắt đầu cập nhật đơn hàng với ID = {OrderId}", id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Không tìm thấy đơn hàng với ID = {OrderId} để cập nhật", id);
            return NotFound();
        }

        _logger.LogInformation("Đơn hàng với ID = {OrderId} đã được cập nhật thành công", id);
        return Ok(result);
    }

    [HttpPut("{id:int}/cancel")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<bool>> CancelOrder(int id, [FromBody] string? cancellationReason = null)
    {
        _logger.LogInformation("Bắt đầu hủy đơn hàng với ID = {OrderId}", id);
        var command = new CancelOrderCommand(id, cancellationReason);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Không thể hủy đơn hàng với ID = {OrderId}", id);
            return BadRequest("Không thể hủy đơn hàng. Có thể đơn hàng không tồn tại hoặc đã được giao.");
        }

        _logger.LogInformation("Đơn hàng với ID = {OrderId} đã được hủy thành công", id);
        return Ok(result);
    }

    [HttpGet("{id}/status-history")]
    [ProducesResponseType(typeof(IEnumerable<OrderStatusHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderStatusHistory(int id)
    {
        _logger.LogInformation("Lấy lịch sử trạng thái đơn hàng {OrderId}", id);
        
        try
        {
            // Kiểm tra xem đơn hàng có tồn tại không
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Không tìm thấy đơn hàng với ID {OrderId}", id);
                return NotFound(new { Message = $"Không tìm thấy đơn hàng với ID {id}" });
            }
            
            // Lấy lịch sử trạng thái
            var statusHistory = await _unitOfWork.OrderRepository.GetOrderStatusHistoryAsync(id);
            
            // Chuyển đổi sang DTO
            var result = statusHistory.Select(h => new OrderStatusHistoryDto
            {
                Id = h.Id,
                OrderId = h.OrderId,
                PreviousStatus = h.PreviousStatus.ToString(),
                NewStatus = h.NewStatus.ToString(),
                ChangedDate = h.ChangedDate,
                Reason = h.Reason,
                ChangedBy = h.ChangedBy
            });
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy lịch sử trạng thái đơn hàng {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Đã xảy ra lỗi khi lấy lịch sử trạng thái đơn hàng" });
        }
    }

    [HttpGet("{id}/possible-transitions")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPossibleTransitions(int id)
    {
        _logger.LogInformation("Lấy các trạng thái có thể chuyển đến cho đơn hàng {OrderId}", id);
        
        try
        {
            // Kiểm tra xem đơn hàng có tồn tại không
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Không tìm thấy đơn hàng với ID {OrderId}", id);
                return NotFound(new { Message = $"Không tìm thấy đơn hàng với ID {id}" });
            }
            
            // Lấy các trạng thái có thể chuyển đến
            var possibleTransitions = _unitOfWork.OrderStateMachine.GetPossibleTransitions(order.Status);
            
            // Chuyển đổi sang chuỗi
            var result = possibleTransitions.Select(s => s.ToString());
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy các trạng thái có thể chuyển đến cho đơn hàng {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Đã xảy ra lỗi khi lấy các trạng thái có thể chuyển đến" });
        }
    }

    [HttpPost("{id}/transition")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TransitionOrderStatus(int id, [FromBody] OrderStatusTransitionDto request)
    {
        _logger.LogInformation("Chuyển trạng thái đơn hàng {OrderId} sang {NewStatus}", id, request.NewStatus);
        
        try
        {
            // Kiểm tra xem đơn hàng có tồn tại không
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Không tìm thấy đơn hàng với ID {OrderId}", id);
                return NotFound(new { Message = $"Không tìm thấy đơn hàng với ID {id}" });
            }
            
            // Chuyển đổi chuỗi trạng thái thành enum
            if (!Enum.TryParse<OrderStatus>(request.NewStatus, true, out var newStatus))
            {
                _logger.LogWarning("Trạng thái không hợp lệ: {NewStatus}", request.NewStatus);
                return BadRequest(new { Message = $"Trạng thái không hợp lệ: {request.NewStatus}" });
            }
            
            // Kiểm tra xem có thể chuyển trạng thái không
            if (!_unitOfWork.OrderStateMachine.CanTransitionTo(order.Status, newStatus))
            {
                _logger.LogWarning("Không thể chuyển từ trạng thái {CurrentStatus} sang {NewStatus}", 
                    order.Status, newStatus);
                return BadRequest(new { 
                    Message = $"Không thể chuyển từ trạng thái {order.Status} sang {newStatus}",
                    PossibleTransitions = _unitOfWork.OrderStateMachine.GetPossibleTransitions(order.Status)
                        .Select(s => s.ToString())
                });
            }
            
            // Thực hiện chuyển trạng thái sử dụng execution strategy
            var strategy = _unitOfWork.CreateExecutionStrategy();
            var result = await strategy.ExecuteAsync(async () => 
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var transitionResult = await _unitOfWork.OrderStateMachine.TransitionToAsync(
                        order, 
                        newStatus, 
                        request.Reason, 
                        request.ChangedBy);
                        
                    if (!transitionResult)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return false;
                    }
                    
                    await _unitOfWork.OrderRepository.UpdateAsync(order);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
            
            if (!result)
            {
                _logger.LogWarning("Không thể chuyển trạng thái đơn hàng {OrderId}", id);
                return BadRequest(new { Message = "Không thể chuyển trạng thái đơn hàng" });
            }
            
            _logger.LogInformation("Đơn hàng {OrderId} đã được chuyển sang trạng thái {NewStatus}", id, newStatus);
            
            return Ok(new { 
                Success = true, 
                Message = $"Đơn hàng đã được chuyển sang trạng thái {newStatus}",
                CurrentStatus = newStatus.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi chuyển trạng thái đơn hàng {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { Message = "Đã xảy ra lỗi khi chuyển trạng thái đơn hàng" });
        }
    }
} 