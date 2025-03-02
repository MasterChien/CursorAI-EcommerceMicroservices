using AutoMapper;
using Catalog.API.DTOs;
using Catalog.Core.Entities;
using Catalog.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CategoryDto>(category));
    }

    [HttpGet("with-products")]
    public async Task<ActionResult<IEnumerable<CategoryWithProductsDto>>> GetCategoriesWithProducts()
    {
        var categories = await _categoryRepository.GetCategoriesWithProductsAsync();
        return Ok(_mapper.Map<IEnumerable<CategoryWithProductsDto>>(categories));
    }

    [HttpGet("{id}/with-products")]
    public async Task<ActionResult<CategoryWithProductsDto>> GetCategoryWithProducts(int id)
    {
        var category = await _categoryRepository.GetCategoryByIdWithProductsAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CategoryWithProductsDto>(category));
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryByName(string name)
    {
        var category = await _categoryRepository.GetCategoryByNameAsync(name);
        if (category == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CategoryDto>(category));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var category = new Category(createCategoryDto.Name, createCategoryDto.Description);
        var createdCategory = await _categoryRepository.AddAsync(category);
        var categoryDto = _mapper.Map<CategoryDto>(createdCategory);

        return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.Update(updateCategoryDto.Name, updateCategoryDto.Description);
        await _categoryRepository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        await _categoryRepository.DeleteAsync(category);
        return NoContent();
    }
} 