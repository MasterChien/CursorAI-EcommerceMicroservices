using AutoMapper;
using Catalog.API.DTOs;
using Catalog.Core.Entities;
using Catalog.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public BrandsController(IBrandRepository brandRepository, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
    {
        var brands = await _brandRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<BrandDto>>(brands));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BrandDto>> GetBrand(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<BrandDto>(brand));
    }

    [HttpGet("with-products")]
    public async Task<ActionResult<IEnumerable<BrandWithProductsDto>>> GetBrandsWithProducts()
    {
        var brands = await _brandRepository.GetBrandsWithProductsAsync();
        return Ok(_mapper.Map<IEnumerable<BrandWithProductsDto>>(brands));
    }

    [HttpGet("{id}/with-products")]
    public async Task<ActionResult<BrandWithProductsDto>> GetBrandWithProducts(int id)
    {
        var brand = await _brandRepository.GetBrandByIdWithProductsAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<BrandWithProductsDto>(brand));
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<BrandDto>> GetBrandByName(string name)
    {
        var brand = await _brandRepository.GetBrandByNameAsync(name);
        if (brand == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<BrandDto>(brand));
    }

    [HttpPost]
    public async Task<ActionResult<BrandDto>> CreateBrand(CreateBrandDto createBrandDto)
    {
        var brand = new Brand(createBrandDto.Name, createBrandDto.Description);
        var createdBrand = await _brandRepository.AddAsync(brand);
        var brandDto = _mapper.Map<BrandDto>(createdBrand);

        return CreatedAtAction(nameof(GetBrand), new { id = brandDto.Id }, brandDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBrand(int id, UpdateBrandDto updateBrandDto)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        brand.Update(updateBrandDto.Name, updateBrandDto.Description);
        await _brandRepository.UpdateAsync(brand);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return NotFound();
        }

        await _brandRepository.DeleteAsync(brand);
        return NoContent();
    }
} 