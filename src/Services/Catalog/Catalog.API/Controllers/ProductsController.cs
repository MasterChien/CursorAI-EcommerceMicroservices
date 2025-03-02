using AutoMapper;
using Catalog.API.DTOs;
using Catalog.Core.Entities;
using Catalog.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productRepository.GetProductByIdWithBrandAndCategoryAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ProductDto>(product));
    }

    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(int brandId)
    {
        var products = await _productRepository.GetProductsByBrandAsync(brandId);
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpGet("search/{name}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByName(string name)
    {
        var products = await _productRepository.GetProductsByNameAsync(name);
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        var createdProduct = await _productRepository.AddAsync(product);
        var productDto = _mapper.Map<ProductDto>(createdProduct);

        return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.UpdateProduct(
            updateProductDto.Name,
            updateProductDto.Description,
            updateProductDto.Price,
            updateProductDto.StockQuantity);

        await _productRepository.UpdateAsync(product);
        return NoContent();
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.UpdateStock(quantity);
        await _productRepository.UpdateAsync(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productRepository.DeleteAsync(product);
        return NoContent();
    }
} 