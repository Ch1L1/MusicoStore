using AutoMapper;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.WebApi.Models.Dtos;

namespace MusicoStore.WebApi.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<Address, AddressDto>()
            .ForMember(d => d.AddressId, o => o.MapFrom(s => s.Id));

        CreateMap<ProductCategory, ProductCategoryDto>()
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products));

        CreateMap<Manufacturer, ManufacturerDto>()
            .ForMember(d => d.ManufacturerId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products));

        CreateMap<ProductCategory, CategorySummaryDto>()
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Name));

        CreateMap<Manufacturer, ManufacturerSummaryDto>()
            .ForMember(d => d.ManufacturerId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ManufacturerName, o => o.MapFrom(s => s.Name));

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.ProductCategory))
            .ForMember(d => d.Manufacturer, o => o.MapFrom(s => s.Manufacturer));

        CreateMap<Product, ProductSummaryForCategoryDto>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ManufacturerName, o => o.MapFrom(s => s.Manufacturer != null ? s.Manufacturer.Name : null));

        CreateMap<Product, ProductSummaryForManufacturerDto>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.ProductCategory != null ? s.ProductCategory.Name : null));
    }
}
