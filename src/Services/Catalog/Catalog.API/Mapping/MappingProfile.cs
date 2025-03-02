using AutoMapper;
using Catalog.API.DTOs;
using Catalog.Core.Entities;

namespace Catalog.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Category mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<Category, CategoryWithProductsDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Brand mappings
        CreateMap<Brand, BrandDto>();
        CreateMap<Brand, BrandWithProductsDto>();
        CreateMap<CreateBrandDto, Brand>();
        CreateMap<UpdateBrandDto, Brand>();
    }
} 