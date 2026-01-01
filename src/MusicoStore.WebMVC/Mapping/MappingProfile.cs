using AutoMapper;
using MusicoStore.Domain.DTOs.Product;
using WebMVC.Models.Products;

namespace WebMVC.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductDTO, ProductViewModel>()
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.ManufacturerName,
                o => o.MapFrom(s => s.Manufacturer != null ? s.Manufacturer.ManufacturerName : null))
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.Category != null ? s.Category.CategoryName : null));
    }
}
