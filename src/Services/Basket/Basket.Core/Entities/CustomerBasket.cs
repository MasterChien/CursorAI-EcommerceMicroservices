using System;
using System.Collections.Generic;

namespace Basket.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {
            Items = new List<BasketItem>();
        }

        public CustomerBasket(string customerId)
        {
            CustomerId = customerId;
            Items = new List<BasketItem>();
        }

        public string CustomerId { get; set; }
        public List<BasketItem> Items { get; set; }
    }
} 