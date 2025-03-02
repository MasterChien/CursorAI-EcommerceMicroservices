using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Basket.Application.DTOs;
using Basket.Core.Entities;
using Basket.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            IBasketRepository repository, 
            IMapper mapper,
            ILogger<BasketController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{customerId}", Name = "GetBasket")]
        [ProducesResponseType(typeof(CustomerBasketDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasketDto>> GetBasket(string customerId)
        {
            var basket = await _repository.GetBasketAsync(customerId);
            
            if (basket == null)
            {
                _logger.LogInformation($"Basket with customerId: {customerId} not found.");
                return Ok(new CustomerBasketDto { CustomerId = customerId });
            }
            
            return Ok(_mapper.Map<CustomerBasketDto>(basket));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasketDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasketDto>> UpdateBasket([FromBody] CustomerBasketDto basketDto)
        {
            var basket = _mapper.Map<CustomerBasket>(basketDto);
            var updatedBasket = await _repository.UpdateBasketAsync(basket);
            
            return Ok(_mapper.Map<CustomerBasketDto>(updatedBasket));
        }

        [HttpDelete("{customerId}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string customerId)
        {
            await _repository.DeleteBasketAsync(customerId);
            return Ok();
        }
        
        [HttpPatch("{customerId}/items")]
        [ProducesResponseType(typeof(CustomerBasketDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CustomerBasketDto>> UpdateQuantity(string customerId, [FromBody] UpdateQuantityDto updateQuantityDto)
        {
            // Lấy giỏ hàng hiện tại
            var basket = await _repository.GetBasketAsync(customerId);
            
            if (basket == null)
            {
                _logger.LogWarning($"Basket with customerId: {customerId} not found.");
                return NotFound($"Basket with customerId: {customerId} not found.");
            }
            
            // Tìm sản phẩm trong giỏ hàng
            var item = basket.Items.FirstOrDefault(i => i.Id == updateQuantityDto.BasketItemId);
            
            if (item == null)
            {
                _logger.LogWarning($"Item with id: {updateQuantityDto.BasketItemId} not found in basket.");
                return BadRequest($"Item with id: {updateQuantityDto.BasketItemId} not found in basket.");
            }
            
            // Cập nhật số lượng
            if (updateQuantityDto.Quantity <= 0)
            {
                // Nếu số lượng <= 0, xóa sản phẩm khỏi giỏ hàng
                basket.Items.Remove(item);
            }
            else
            {
                // Cập nhật số lượng
                item.Quantity = updateQuantityDto.Quantity;
            }
            
            // Lưu giỏ hàng đã cập nhật
            var updatedBasket = await _repository.UpdateBasketAsync(basket);
            
            return Ok(_mapper.Map<CustomerBasketDto>(updatedBasket));
        }
    }
} 