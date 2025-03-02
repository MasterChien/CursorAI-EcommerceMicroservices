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
using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(
            IInventoryService inventoryService,
            IMapper mapper,
            ILogger<WarehouseController> logger)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WarehouseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAllWarehouses()
        {
            var warehouses = await _inventoryService.GetAllWarehousesAsync();
            return Ok(_mapper.Map<IEnumerable<WarehouseDto>>(warehouses));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseDto>> GetWarehouseById(int id)
        {
            _logger.LogInformation("Lấy thông tin kho theo Id: {Id}", id);
            var warehouse = await _inventoryService.GetWarehouseByIdAsync(id);
            return Ok(_mapper.Map<WarehouseDto>(warehouse));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<WarehouseDto>> UpdateWarehouse(int id, [FromBody] UpdateWarehouseRequest request)
        {
            try
            {
                _logger.LogInformation("Cập nhật thông tin kho: {Id}", id);
                
                // Validation cơ bản
                if (request == null)
                    return BadRequest("Dữ liệu yêu cầu không hợp lệ");

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { error = "Tên kho không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest(new { error = "Mã kho không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Address))
                    return BadRequest(new { error = "Địa chỉ không được để trống" });

                if (string.IsNullOrWhiteSpace(request.City))
                    return BadRequest(new { error = "Thành phố không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Country))
                    return BadRequest(new { error = "Quốc gia không được để trống" });

                var warehouse = new Warehouse
                {
                    Name = request.Name,
                    Code = request.Code,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    ContactPerson = request.ContactPerson,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    IsActive = request.IsActive
                };

                var result = await _inventoryService.UpdateWarehouseAsync(id, warehouse);
                var resultDto = _mapper.Map<WarehouseDto>(result);
                
                return Ok(resultDto);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy kho: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Lỗi validation khi cập nhật kho: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Lỗi khi cập nhật kho: {Message}", ex.Message);
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi cập nhật kho: {Message}", ex.Message);
                throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteWarehouse(int id)
        {
            try
            {
                _logger.LogInformation("Xóa kho: {Id}", id);
                await _inventoryService.DeleteWarehouseAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Không tìm thấy kho: {Message}", ex.Message);
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Không thể xóa kho: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi xóa kho: {Message}", ex.Message);
                throw;
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse([FromBody] CreateWarehouseRequest request)
        {
            try
            {
                // Validation cơ bản
                if (request == null)
                    return BadRequest("Dữ liệu yêu cầu không hợp lệ");

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { error = "Tên kho không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest(new { error = "Mã kho không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Address))
                    return BadRequest(new { error = "Địa chỉ không được để trống" });

                if (string.IsNullOrWhiteSpace(request.City))
                    return BadRequest(new { error = "Thành phố không được để trống" });

                if (string.IsNullOrWhiteSpace(request.Country))
                    return BadRequest(new { error = "Quốc gia không được để trống" });

                var warehouse = new Warehouse
                {
                    Name = request.Name,
                    Code = request.Code,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    ContactPerson = request.ContactPerson,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    IsActive = true
                };

                var result = await _inventoryService.CreateWarehouseAsync(warehouse);
                var resultDto = _mapper.Map<WarehouseDto>(result);
                
                return CreatedAtAction(nameof(GetWarehouseById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Lỗi validation khi tạo kho: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Lỗi khi tạo kho: {Message}", ex.Message);
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi tạo kho: {Message}", ex.Message);
                throw;
            }
        }

        [HttpGet("{warehouseId}/stock")]
        [ProducesResponseType(typeof(IEnumerable<WarehouseItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WarehouseItemDto>>> GetStockByWarehouse(int warehouseId)
        {
            var items = await _inventoryService.GetStockByWarehouseAsync(warehouseId);
            return Ok(_mapper.Map<IEnumerable<WarehouseItemDto>>(items));
        }
    }

    public class CreateWarehouseRequest
    {
        [Required(ErrorMessage = "Tên kho là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên kho không được vượt quá 100 ký tự")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Mã kho là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã kho không được vượt quá 20 ký tự")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        [StringLength(50, ErrorMessage = "Thành phố không được vượt quá 50 ký tự")]
        public required string City { get; set; }

        [StringLength(50, ErrorMessage = "Tiểu bang không được vượt quá 50 ký tự")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        [StringLength(50, ErrorMessage = "Quốc gia không được vượt quá 50 ký tự")]
        public required string Country { get; set; }

        [StringLength(20, ErrorMessage = "Mã bưu điện không được vượt quá 20 ký tự")]
        public required string ZipCode { get; set; }

        [StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự")]
        public required string ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public required string ContactEmail { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public required string ContactPhone { get; set; }
    }

    public class UpdateWarehouseRequest
    {
        [Required(ErrorMessage = "Tên kho là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên kho không được vượt quá 100 ký tự")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Mã kho là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã kho không được vượt quá 20 ký tự")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        [StringLength(50, ErrorMessage = "Thành phố không được vượt quá 50 ký tự")]
        public required string City { get; set; }

        [StringLength(50, ErrorMessage = "Tiểu bang không được vượt quá 50 ký tự")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        [StringLength(50, ErrorMessage = "Quốc gia không được vượt quá 50 ký tự")]
        public required string Country { get; set; }

        [StringLength(20, ErrorMessage = "Mã bưu điện không được vượt quá 20 ký tự")]
        public required string ZipCode { get; set; }

        [StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự")]
        public required string ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public required string ContactEmail { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public required string ContactPhone { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
} 