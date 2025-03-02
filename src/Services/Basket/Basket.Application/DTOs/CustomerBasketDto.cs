using System;
using System.Collections.Generic;

namespace Basket.Application.DTOs
{
    public class CustomerBasketDto
    {
        public string CustomerId { get; set; }
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
    }
} 