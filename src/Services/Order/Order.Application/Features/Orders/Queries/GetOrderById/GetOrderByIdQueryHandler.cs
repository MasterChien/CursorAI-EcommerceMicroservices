using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.DTOs;
using Order.Core.Interfaces;

namespace Order.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOrderByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting order by ID {OrderId}", request.OrderId);

            // Lấy thông tin đơn hàng từ database (kèm theo OrderItems)
            _logger.LogInformation("Gọi GetOrderByIdWithItemsAsync với ID {OrderId}", request.OrderId);
            var order = await _unitOfWork.OrderRepository.GetOrderByIdWithItemsAsync(request.OrderId);
            
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", request.OrderId);
                return null;
            }

            _logger.LogInformation("Tìm thấy đơn hàng với ID {OrderId}, có {ItemCount} items", 
                request.OrderId, order.OrderItems?.Count ?? 0);

            // Map sang DTO
            var orderDto = _mapper.Map<OrderDto>(order);
            _logger.LogInformation("Đã map đơn hàng sang DTO, có {ItemCount} items", orderDto.OrderItems?.Count ?? 0);

            _logger.LogInformation("Retrieved order with ID {OrderId}", request.OrderId);
            return orderDto;
        }
    }
} 