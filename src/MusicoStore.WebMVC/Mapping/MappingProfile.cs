using AutoMapper;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.DTOs.GiftCard;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Entities;
using WebMVC.Models.Address;
using WebMVC.Models.Category;
using WebMVC.Models.GiftCard;
using WebMVC.Models.Manufacturer;
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
            .ForMember(d => d.CategoryNames,
                o => o.MapFrom(s =>
                    s.Categories == null || !s.Categories.Any()
                        ? null
                        : string.Join(", ",
                            s.Categories
                                .OrderByDescending(c => c.IsPrimary)
                                .Select(c => c.Name)
                        )
                ));

        CreateMap<ProductCreateViewModel, CreateProductDTO>()
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.CurrentPrice, o => o.MapFrom(s => s.Price));


        CreateMap<ManufacturerDTO, ManufacturerViewModel>();
        CreateMap<AddressDTO, AddressViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AddressId));

        CreateMap<ManufacturerDTO, ManufacturerFormViewModel>()
            .ForMember(dest => dest.AddressId,
                opt => opt.MapFrom(src => src.Address != null ? src.Address.AddressId : 0))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ManufacturerId));

        CreateMap<ManufacturerFormViewModel, CreateManufacturerDTO>();

        CreateMap<ManufacturerFormViewModel, UpdateManufacturerDTO>();

        CreateMap<ProductCategoryDTO, ProductCategoryViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId));

        CreateMap<ProductCategoryDTO, ProductCategoryFormViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId));

        CreateMap<ProductCategoryFormViewModel, CreateProductCategoryDTO>();

        CreateMap<ProductCategoryFormViewModel, UpdateProductCategoryDTO>();

        CreateMap<GiftCardCreateViewModel, CreateGiftCardDTO>();

        CreateMap<ProductDTO, ProductEditViewModel>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.CurrentPrice))
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Manufacturers, opt => opt.Ignore());

        CreateMap<ProductEditViewModel, UpdateProductDTO>()
            .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.Price));
    }
}
