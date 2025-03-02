using Inventory.Application.DTOs;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Inventory.API.Controllers
{
    [Route("api/inventory/transfers")]
    [ApiController]
    public class StockTransferController : ControllerBase
    {
        private readonly IStockTransferService _stockTransferService;
        private readonly ILogger<StockTransferController> _logger;
        private readonly IMapper _mapper;

        public StockTransferController(IStockTransferService stockTransferService, ILogger<StockTransferController> logger, IMapper mapper)
        {
            _stockTransferService = stockTransferService;
            _logger = logger;
            _mapper = mapper;
        }

        // ... existing code ...

        [HttpPost]
        [ProducesResponseType(typeof(StockTransferDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StockTransferDto>> CreateStockTransfer([FromBody] CreateStockTransferDto createDto)
        {
            try
            {
                var sourceWarehouse = await _stockTransferService.GetWarehouseByIdAsync(createDto.SourceWarehouseId);
                if (sourceWarehouse == null)
                {
                    return BadRequest($"Không tìm thấy kho nguồn với ID {createDto.SourceWarehouseId}");
                }
                
                var destinationWarehouse = await _stockTransferService.GetWarehouseByIdAsync(createDto.DestinationWarehouseId);
                if (destinationWarehouse == null)
                {
                    return BadRequest($"Không tìm thấy kho đích với ID {createDto.DestinationWarehouseId}");
                }
                
                var transfer = new StockTransfer
                {
                    SourceWarehouseId = createDto.SourceWarehouseId,
                    DestinationWarehouseId = createDto.DestinationWarehouseId,
                    SourceWarehouse = sourceWarehouse,
                    DestinationWarehouse = destinationWarehouse,
                    TransferDate = DateTime.UtcNow,
                    Status = StockTransferStatus.Draft,
                    RequestedBy = createDto.RequestedBy,
                    ApprovedBy = string.Empty, // Sẽ được cập nhật khi phê duyệt
                    Notes = createDto.Notes,
                    TransferNumber = $"STF-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}"
                };

                var transferItems = new List<StockTransferItem>();
                foreach (var item in createDto.TransferItems)
                {
                    var inventoryItem = await _stockTransferService.GetInventoryItemByIdAsync(item.InventoryItemId);
                    if (inventoryItem == null)
                    {
                        return BadRequest($"Không tìm thấy mặt hàng với ID {item.InventoryItemId}");
                    }
                    
                    transferItems.Add(new StockTransferItem
                    {
                        InventoryItemId = item.InventoryItemId,
                        InventoryItem = inventoryItem,
                        StockTransfer = transfer,
                        Quantity = item.Quantity,
                        Notes = item.Notes
                    });
                }

                var result = await _stockTransferService.CreateStockTransferAsync(transfer, transferItems);
                var resultDto = _mapper.Map<StockTransferDto>(result);

                return CreatedAtAction(nameof(GetStockTransferById), new { id = result.Id }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phiếu chuyển kho");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StockTransferDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockTransferDto>> GetStockTransferById(int id)
        {
            try
            {
                var transfer = await _stockTransferService.GetStockTransferByIdAsync(id);
                if (transfer == null)
                {
                    return NotFound($"Không tìm thấy phiếu chuyển kho với ID {id}");
                }
                
                var transferDto = _mapper.Map<StockTransferDto>(transfer);
                return Ok(transferDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy thông tin phiếu chuyển kho ID {id}");
                return BadRequest(ex.Message);
            }
        }
    }
} 