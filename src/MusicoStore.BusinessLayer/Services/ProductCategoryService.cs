using AutoMapper;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class ProductCategoryService(IRepository<ProductCategory> productCategoryRepository, IMapper mapper)
    : IProductCategoryService
{
    public async Task<List<ProductCategoryDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<ProductCategory> productCategories = await productCategoryRepository.GetAllAsync(ct);
        return mapper.Map<List<ProductCategoryDTO>>(productCategories);
    }

    public async Task<ProductCategoryDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        ProductCategory? productCategory = await productCategoryRepository.GetByIdAsync(id, ct);
        return mapper.Map<ProductCategoryDTO>(productCategory);
    }

    public bool DoesExistById(int id)
        => productCategoryRepository.DoesEntityExist(id);

    public async Task<ProductCategoryDTO> CreateAsync(CreateProductCategoryDTO dto, CancellationToken ct)
    {
        ProductCategory productCategory = mapper.Map<ProductCategory>(dto);
        ProductCategory created = await productCategoryRepository.AddAsync(productCategory, ct);
        return mapper.Map<ProductCategoryDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateProductCategoryDTO dto, CancellationToken ct)
    {
        ProductCategory productCategory = mapper.Map<ProductCategory>(dto);
        productCategory.Id = id;
        await productCategoryRepository.UpdateAsync(productCategory, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await productCategoryRepository.DeleteAsync(id, ct);
}
