using System;
using System.Text.Json;
using System.Threading.Tasks;
using Basket.Core.Entities;
using Basket.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<CustomerBasket?> GetBasketAsync(string customerId)
        {
            var basket = await _redisCache.GetStringAsync(customerId);
            
            if (string.IsNullOrEmpty(basket))
                return null;

            return JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            await _redisCache.SetStringAsync(basket.CustomerId, JsonSerializer.Serialize(basket));

            return (await GetBasketAsync(basket.CustomerId))!;
        }

        public async Task<bool> DeleteBasketAsync(string customerId)
        {
            await _redisCache.RemoveAsync(customerId);
            return true;
        }
    }
} 