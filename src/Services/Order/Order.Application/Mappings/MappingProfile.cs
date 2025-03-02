using AutoMapper;
using Order.Application.DTOs;
using Order.Core.Entities;
using Order.Application.Features.Orders.Commands.UpdateOrder;

namespace Order.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Core.Entities.Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()));
            
            CreateMap<OrderItem, OrderItemDto>();
            CreateMap<OrderItemDto, OrderItem>();
            
            // UpdateOrder mappings
            CreateMap<OrderItem, UpdateOrderItemDto>();
            CreateMap<UpdateOrderItemDto, OrderItem>();
            
            // Commands and DTOs mapping will be added as needed
        }
    }
} 