using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.DTOs;
using Order.Core.Entities;
using Order.Core.Interfaces;

namespace Order.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrdersListQueryHandler> _logger;

        public GetOrdersListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOrdersListQueryHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<OrderDto>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting orders list");

            IReadOnlyList<Core.Entities.Order> orders;

            // Lọc theo CustomerId nếu có
            if (!string.IsNullOrEmpty(request.CustomerId))
            {
                var customerOrders = await _unitOfWork.OrderRepository.GetOrdersByCustomerIdAsync(request.CustomerId);
                orders = customerOrders.ToList();
            }
            else
            {
                // Lọc theo Status nếu có
                if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var status))
                {
                    var statusOrders = await _unitOfWork.OrderRepository.GetOrdersByStatusAsync(status);
                    orders = statusOrders.ToList();
                }
                else
                {
                    // Lấy tất cả đơn hàng
                    orders = await _unitOfWork.OrderRepository.GetAllAsync();
                }
            }

            // Lọc theo ngày tạo
            if (request.FromDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedDate >= request.FromDate.Value).ToList();
            }

            if (request.ToDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedDate <= request.ToDate.Value).ToList();
            }

            // Phân trang
            var pagedOrders = orders
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map sang DTO
            var orderDtos = _mapper.Map<List<OrderDto>>(pagedOrders);

            _logger.LogInformation("Retrieved {Count} orders", orderDtos.Count);
            return orderDtos;
        }
    }
} 