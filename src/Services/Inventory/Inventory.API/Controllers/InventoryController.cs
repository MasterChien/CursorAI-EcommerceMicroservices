using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Core.Entities;
using Inventory.Core.Exceptions;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IInventoryService inventoryService,
            IMapper mapper,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("items")]
        [ProducesResponseType(typeof(IEnumerable<InventoryItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAllInventoryItems()
        {
            var items = await _inventoryService.GetAllInventoryItemsAsync();
            return Ok(_mapper.Map<IEnumerable<InventoryItemDto>>(items));
        }

        [HttpGet("items/{productId}")]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemDto>> GetInventoryItemByProductId(int productId)
        {
            var item = await _inventoryService.GetInventoryItemByProductIdAsync(productId);
            return Ok(_mapper.Map<InventoryItemDto>(item));
        }

        [HttpPost("items")]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InventoryItemDto>> CreateInventoryItem([FromBody] CreateInventoryItemDto createDto)
        {
            _logger.LogInformation($"Tạo mục kho mới: {createDto.ProductName}");
            var item = _mapper.Map<InventoryItem>(createDto);
            var result = await _inventoryService.CreateInventoryItemAsync(item);
            var resultDto = _mapper.Map<InventoryItemDto>(result);
            return CreatedAtAction(nameof(GetInventoryItemByProductId), new { productId = result.ProductId }, resultDto);
        }

        [HttpGet("stock/{productId}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetTotalStock(int productId)
        {
            var totalStock = await _inventoryService.GetTotalStockAsync(productId);
            return Ok(totalStock);
        }

        [HttpGet("stock/{productId}/details")]
        [ProducesResponseType(typeof(IEnumerable<WarehouseItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WarehouseItemDto>>> GetStockByProduct(int productId)
        {
            var items = await _inventoryService.GetStockByProductAsync(productId);
            return Ok(_mapper.Map<IEnumerable<WarehouseItemDto>>(items));
        }

        [HttpPost("stock/add")]
        [ProducesResponseType(typeof(WarehouseItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseItemDto>> AddStock([FromBody] AddStockRequest request)
        {
            var result = await _inventoryService.AddStockAsync(request.ProductId, request.WarehouseId, request.Quantity);
            return Ok(_mapper.Map<WarehouseItemDto>(result));
        }

        [HttpPost("stock/remove")]
        [ProducesResponseType(typeof(WarehouseItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseItemDto>> RemoveStock([FromBody] RemoveStockRequest request)
        {
            var result = await _inventoryService.RemoveStockAsync(request.ProductId, request.WarehouseId, request.Quantity);
            return Ok(_mapper.Map<WarehouseItemDto>(result));
        }

        [HttpGet("stock/check")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> CheckStock([FromQuery] int productId, [FromQuery] int quantity)
        {
            var isInStock = await _inventoryService.IsInStockAsync(productId, quantity);
            return Ok(isInStock);
        }

        [HttpPost("stock/reserve")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> ReserveStock([FromBody] ReserveStockRequest request)
        {
            try
            {
                _logger.LogInformation("Đặt trước hàng: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
                    request.ProductId, request.WarehouseId, request.Quantity);

                var result = await _inventoryService.ReserveStockAsync(request.ProductId, request.WarehouseId, request.Quantity);
                return Ok(result);
            }
            catch (InventoryItemNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mặt hàng: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (WarehouseNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy kho: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InsufficientStockException ex)
            {
                _logger.LogWarning(ex, "Không đủ hàng để đặt trước: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi đặt trước hàng: {Message}", ex.Message);
                throw;
            }
        }

        [HttpPost("stock/release")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> ReleaseReservedStock([FromBody] ReleaseReservedStockRequest request)
        {
            try
            {
                _logger.LogInformation("Giải phóng hàng đặt trước: ProductId={ProductId}, WarehouseId={WarehouseId}, Quantity={Quantity}",
                    request.ProductId, request.WarehouseId, request.Quantity);

                var result = await _inventoryService.ReleaseReservedStockAsync(request.ProductId, request.WarehouseId, request.Quantity);
                return Ok(result);
            }
            catch (InventoryItemNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mặt hàng: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (WarehouseNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy kho: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Lỗi khi giải phóng hàng đặt trước: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi giải phóng hàng đặt trước: {Message}", ex.Message);
                throw;
            }
        }

        [HttpGet("transactions/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<InventoryTransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> GetTransactionHistory(
            int productId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var transactions = await _inventoryService.GetTransactionHistoryAsync(productId, startDate, endDate);
            return Ok(_mapper.Map<IEnumerable<InventoryTransactionDto>>(transactions));
        }

        [HttpGet("stock/{productId}/reserved")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetReservedStock(int productId)
        {
            try
            {
                _logger.LogInformation("Lấy thông tin số lượng đặt trước: ProductId={ProductId}", productId);
                var reservedStock = await _inventoryService.GetReservedStockAsync(productId);
                return Ok(reservedStock);
            }
            catch (InventoryItemNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mặt hàng: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy thông tin đặt trước: {Message}", ex.Message);
                throw;
            }
        }

        [HttpGet("items/low-stock")]
        [ProducesResponseType(typeof(IEnumerable<InventoryItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetLowStockItems()
        {
            try
            {
                _logger.LogInformation("Lấy danh sách sản phẩm có mức tồn kho thấp");
                var items = await _inventoryService.GetLowStockItemsAsync();
                return Ok(_mapper.Map<IEnumerable<InventoryItemDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách sản phẩm có mức tồn kho thấp: {Message}", ex.Message);
                throw;
            }
        }

        [HttpPut("items/{productId}")]
        [ProducesResponseType(typeof(InventoryItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemDto>> UpdateInventoryItem(int productId, [FromBody] UpdateInventoryItemDto updateDto)
        {
            try
            {
                _logger.LogInformation("Cập nhật thông tin mục kho: ProductId={ProductId}", productId);
                
                var existingItem = await _inventoryService.GetInventoryItemByProductIdAsync(productId);
                if (existingItem == null)
                {
                    return NotFound(new { error = $"Không tìm thấy mục kho với ProductId {productId}" });
                }

                if (!string.IsNullOrEmpty(updateDto.ProductName))
                {
                    existingItem.ProductName = updateDto.ProductName;
                }

                if (!string.IsNullOrEmpty(updateDto.SKU))
                {
                    existingItem.SKU = updateDto.SKU;
                }

                if (updateDto.LowStockThreshold.HasValue)
                {
                    existingItem.LowStockThreshold = updateDto.LowStockThreshold.Value;
                }

                var updatedItem = await _inventoryService.UpdateInventoryItemAsync(productId, existingItem);
                return Ok(_mapper.Map<InventoryItemDto>(updatedItem));
            }
            catch (InventoryItemNotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy mặt hàng: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi cập nhật thông tin mục kho: {Message}", ex.Message);
                throw;
            }
        }
    }

    public class AddStockRequest
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveStockRequest
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReserveStockRequest
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReleaseReservedStockRequest
    {
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
    }
} 