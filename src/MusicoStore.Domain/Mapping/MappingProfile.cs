using AutoMapper;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.DTOs.Customer;
using MusicoStore.Domain.DTOs.CustomerAddress;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.DTOs.Stock;
using MusicoStore.Domain.DTOs.Storage;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Records;

namespace MusicoStore.Domain.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Address, AddressDTO>()
            .ForMember(d => d.AddressId, o => o.MapFrom(s => s.Id));

        CreateMap<CreateAddressDTO, Address>();

        CreateMap<Manufacturer, ManufacturerDTO>()
            .ForMember(d => d.ManufacturerId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products));

        CreateMap<Manufacturer, ManufacturerSummaryDTO>()
            .ForMember(d => d.ManufacturerId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ManufacturerName, o => o.MapFrom(s => s.Name));

        CreateMap<CreateManufacturerDTO, Manufacturer>();
        CreateMap<UpdateManufacturerDTO, Manufacturer>();

        CreateMap<Product, ProductDTO>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.ProductCategory))
            .ForMember(d => d.Manufacturer, o => o.MapFrom(s => s.Manufacturer))
            .ForMember(d => d.ImagePath, o => o.MapFrom(s => s.ImagePath))
            .ForMember(d => d.EditCount, o => o.MapFrom(s => s.EditLogs.Count))
            .ForMember(d => d.LastEditedByCustomerId,
                o => o.MapFrom(s => s.EditLogs.LastOrDefault() != null
                    ? s.EditLogs.LastOrDefault()!.CustomerId
                    : 0));


        CreateMap<Product, ProductSummaryForCategoryDTO>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ManufacturerName,
                o => o.MapFrom(s => s.Manufacturer != null ? s.Manufacturer.Name : null));

        CreateMap<Product, ProductSummaryForManufacturerDTO>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.ProductCategory != null ? s.ProductCategory.Name : null));

        CreateMap<CreateProductDTO, Product>();
        CreateMap<UpdateProductDTO, Product>();

        CreateMap<ProductCategory, ProductCategoryDTO>()
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Products, o => o.MapFrom(s => s.Products));

        CreateMap<ProductCategory, ProductCategorySummaryDTO>()
            .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Name));

        CreateMap<CreateProductCategoryDTO, ProductCategory>();
        CreateMap<UpdateProductCategoryDTO, ProductCategory>();

        CreateMap<ProductFilterRequestDTO, ProductFilterCriteria>();


        CreateMap<Storage, StorageDTO>()
            .ForMember(d => d.StorageId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Address, o => o.MapFrom(s => s.Address));

        CreateMap<Storage, StorageSummaryForStockDTO>()

            .ForMember(d => d.Address, o => o.MapFrom(s => s.Address));

        CreateMap<CreateStorageDTO, Storage>();
        CreateMap<UpdateStorageDTO, Storage>();

        CreateMap<Stock, StockSummaryForProductDTO>()
            .ForMember(d => d.Storage, o => o.MapFrom(s => s.Storage))
            .ForMember(d => d.CurrentQuantity, o => o.MapFrom(s => s.CurrentQuantity));

        CreateMap<Stock, StockSummaryForStorageDTO>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product))
            .ForMember(d => d.CurrentQuantity, o => o.MapFrom(s => s.CurrentQuantity));

        CreateMap<StockUpdateDTO, Stock>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
            .ForMember(d => d.CurrentQuantity, o => o.MapFrom(s => s.QuantityDifference));


        CreateMap<Product, ProductSummaryForStockDTO>()
            .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ManufacturerName,
                o => o.MapFrom(s => s.Manufacturer != null ? s.Manufacturer.Name : null))
            .ForMember(d => d.CategoryName,
                o => o.MapFrom(s => s.ProductCategory != null ? s.ProductCategory.Name : null));

        CreateMap<Customer, CustomerDTO>()
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.IsEmployee))
            .ReverseMap();

        CreateMap<CreateCustomerDTO, Customer>()
            .ForMember(dest => dest.IsEmployee, opt => opt.MapFrom(src => src.Employee));

        CreateMap<UpdateCustomerDTO, Customer>()
            .ForMember(dest => dest.IsEmployee, opt => opt.MapFrom(src => src.Employee));

        CreateMap<CustomerAddress, CustomerAddressSummaryForCustomerDTO>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<OrderedProduct, OrderItemDTO>()
            .ForMember(d => d.ProductName,
                opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(d => d.LineTotal,
                opt => opt.MapFrom(src => src.PricePerItem * src.Quantity));

        CreateMap<Order, OrderDTO>()
        .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.Id))
        .ForMember(d => d.CreatedAt, opt => opt.Ignore())
        .ForMember(d => d.CurrentState, opt => opt.Ignore())
        .ForMember(d => d.TotalAmount, opt => opt.Ignore())
        .ForMember(d => d.Items, opt => opt.Ignore());

        CreateMap<CreateOrderDTO, Order>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Customer, opt => opt.Ignore())
            .ForMember(d => d.CustomerAddress, opt => opt.Ignore())
            .ForMember(d => d.OrderedProducts, opt => opt.Ignore())
            .ForMember(d => d.StatusLog, opt => opt.Ignore());

        CreateMap<CreateOrderItemDTO, OrderedProduct>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Order, opt => opt.Ignore())
            .ForMember(d => d.Product, opt => opt.Ignore());
    }
}
