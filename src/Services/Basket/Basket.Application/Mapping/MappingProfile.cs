using AutoMapper;
using Basket.Application.DTOs;
using Basket.Core.Entities;

namespace Basket.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();
        }
    }
} 