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
    [Route("api/inventory/counts")]
    public class InventoryCountController : ControllerBase
    {
        private readonly IInventoryCountService _inventoryCountService;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryCountController> _logger;

        public InventoryCountController(
            IInventoryCountService inventoryCountService,
            IMapper mapper,
            ILogger<InventoryCountController> logger)
        {
            _inventoryCountService = inventoryCountService ?? throw new ArgumentNullException(nameof(inventoryCountService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InventoryCountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryCountDto>>> GetAllInventoryCounts()
        {
            var counts = await _inventoryCountService.GetAllInventoryCountsAsync();
            return Ok(_mapper.Map<IEnumerable<InventoryCountDto>>(counts));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountDto>> GetInventoryCountById(int id)
        {
            try
            {
                var count = await _inventoryCountService.GetInventoryCountByIdAsync(id);
                return Ok(_mapper.Map<InventoryCountDto>(count));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("warehouse/{warehouseId}")]
        [ProducesResponseType(typeof(IEnumerable<InventoryCountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryCountDto>>> GetInventoryCountsByWarehouse(int warehouseId)
        {
            var counts = await _inventoryCountService.GetInventoryCountsByWarehouseAsync(warehouseId);
            return Ok(_mapper.Map<IEnumerable<InventoryCountDto>>(counts));
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<InventoryCountDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryCountDto>>> GetInventoryCountsByStatus(string status)
        {
            if (!Enum.TryParse<InventoryCountStatus>(status, true, out var countStatus))
            {
                return BadRequest(new { error = $"Trạng thái không hợp lệ: {status}" });
            }

            var counts = await _inventoryCountService.GetInventoryCountsByStatusAsync(countStatus);
            return Ok(_mapper.Map<IEnumerable<InventoryCountDto>>(counts));
        }

        [HttpPost]
        [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InventoryCountDto>> CreateInventoryCount([FromBody] CreateInventoryCountDto createDto)
        {
            try
            {
                var count = await _inventoryCountService.CreateInventoryCountAsync(
                    createDto.WarehouseId,
                    createDto.CountBy,
                    createDto.Notes);

                var resultDto = _mapper.Map<InventoryCountDto>(count);
                return CreatedAtAction(nameof(GetInventoryCountById), new { id = count.Id }, resultDto);
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/items")]
        [ProducesResponseType(typeof(IEnumerable<InventoryCountItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryCountItemDto>>> GetInventoryCountItems(int id)
        {
            try
            {
                var countItems = await _inventoryCountService.GetInventoryCountItemsAsync(id);
                return Ok(_mapper.Map<IEnumerable<InventoryCountItemDto>>(countItems));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/items")]
        [ProducesResponseType(typeof(InventoryCountItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountItemDto>> UpdateInventoryCountItem(
            int id, [FromBody] UpdateInventoryCountItemDto updateDto)
        {
            try
            {
                var countItem = await _inventoryCountService.UpdateInventoryCountItemAsync(
                    id,
                    updateDto.InventoryItemId,
                    updateDto.ActualQuantity,
                    updateDto.Notes);

                return Ok(_mapper.Map<InventoryCountItemDto>(countItem));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/start")]
        [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountDto>> StartInventoryCount(int id)
        {
            try
            {
                var count = await _inventoryCountService.StartInventoryCountAsync(id);
                return Ok(_mapper.Map<InventoryCountDto>(count));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountDto>> CompleteInventoryCount(
            int id, [FromBody] CompleteInventoryCountDto completeDto)
        {
            try
            {
                // Lấy thông tin hiện tại của kiểm kê
                var countItems = await _inventoryCountService.GetInventoryCountItemsAsync(id);
                var countItemsMap = new Dictionary<int, InventoryCountItem>();
                foreach (var item in countItems)
                {
                    countItemsMap[item.InventoryItemId] = item;
                }

                // Cập nhật thông tin kiểm kê từ request
                foreach (var updateItem in completeDto.CountItems)
                {
                    if (countItemsMap.TryGetValue(updateItem.InventoryItemId, out var countItem))
                    {
                        countItem.ActualQuantity = updateItem.ActualQuantity;
                        countItem.Notes = updateItem.Notes;
                    }
                }

                var count = await _inventoryCountService.CompleteInventoryCountAsync(
                    id,
                    countItemsMap.Values,
                    completeDto.Notes);

                return Ok(_mapper.Map<InventoryCountDto>(count));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(InventoryCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryCountDto>> CancelInventoryCount(
            int id, [FromBody] string reason)
        {
            try
            {
                var count = await _inventoryCountService.CancelInventoryCountAsync(id, reason);
                return Ok(_mapper.Map<InventoryCountDto>(count));
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/discrepancy-summary")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dictionary<string, int>>> GetDiscrepancySummary(int id)
        {
            try
            {
                var summary = await _inventoryCountService.GetInventoryDiscrepancySummaryAsync(id);
                return Ok(summary);
            }
            catch (Exception ex) when (ex is InventoryException)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
} 