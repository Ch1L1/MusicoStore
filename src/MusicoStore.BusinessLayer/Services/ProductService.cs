using AutoMapper;
using MusicoStore.Domain.Constants;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Records;

namespace MusicoStore.BusinessLayer.Services;

public class ProductService(
    IProductRepository productRepository,
    IMapper mapper,
    IRepository<Customer> customerRepository,
    IRepository<ProductEditLog> productEditLogRepository,
    IFileStorageService fileStorageService
    ) : IProductService
{
    public async Task<List<ProductDTO>> FilterAsync(ProductFilterRequestDTO filterDto, CancellationToken ct)
    {
        ProductFilterCriteria filter = mapper.Map<ProductFilterCriteria>(filterDto);
        IReadOnlyList<Product> products = await productRepository.FilterAsync(filter, ct);
        return mapper.Map<List<ProductDTO>>(products);
    }

    public async Task<PagedResult<ProductDTO>> FilterPagedAsync(ProductFilterRequestDTO filterDto, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        ProductFilterCriteria filter = mapper.Map<ProductFilterCriteria>(filterDto);
        var (items, totalCount) = await productRepository.FilterPagedAsync(filter, page, pageSize, ct);
        return new PagedResult<ProductDTO>
        {
            Items = mapper.Map<IEnumerable<ProductDTO>>(items),
            TotalCount = totalCount
        };
    }

    public async Task<ProductDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(id, ct);
        return mapper.Map<ProductDTO>(product);
    }

    public bool DoesExistById(int id)
        => productRepository.DoesEntityExist(id);

    public async Task<ProductDTO> CreateAsync(CreateProductDTO dto, int editedByCustomerId, CancellationToken ct)
    {
        Customer editor = await EnsureEditorIsEmployeeAsync(editedByCustomerId, ct);
        Product product = mapper.Map<Product>(dto);
        Product created = await productRepository.AddAsync(product, ct);
        await LogEditAsync(created.Id, editor.Id, ct);
        return mapper.Map<ProductDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateProductDTO dto, int editedByCustomerId, CancellationToken ct)
    {
        Customer editor = await EnsureEditorIsEmployeeAsync(editedByCustomerId, ct);
        Product product = mapper.Map<Product>(dto);
        product.Id = id;
        await productRepository.UpdateAsync(product, ct);
        await LogEditAsync(id, editor.Id, ct);
    }

    public async Task DeleteByIdAsync(int id, int editedByCustomerId, CancellationToken ct)
    {
        Customer editor = await EnsureEditorIsEmployeeAsync(editedByCustomerId, ct);
        Product? product = await productRepository.GetByIdWithoutLogsAsync(id, ct);
        
        if (product != null)
        {
            fileStorageService.DeleteFile(product.ImagePath);
            await productRepository.DeleteAsync(id, ct);
            await LogEditAsync(id, editor.Id, ct);
        } else
        {
            throw new KeyNotFoundException(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }
    }

    public async Task<string> UploadImageAsync(int id, FileDTO fileDto, int editedByCustomerId, CancellationToken ct)
    {
        Customer editor = await EnsureEditorIsEmployeeAsync(editedByCustomerId, ct);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(fileDto.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"Unsupported file type: {extension}");
        }

        Product? product = await productRepository.GetByIdWithoutLogsAsync(id, ct);
        if (product is null)
        {
            throw new KeyNotFoundException(string.Format(ErrorMessages.NotFoundFormat, "Product",  $"id '{id}'"));
        }

        if (!string.IsNullOrEmpty(product.ImagePath))
        {
            fileStorageService.DeleteFile(product.ImagePath);
        }
        
        var relativePath = await fileStorageService.SaveFileAsync(fileDto, "images/products", ct);

        product.ImagePath = relativePath;
        await productRepository.UpdateAsync(product, ct);
        await LogEditAsync(id, editor.Id, ct);

        return relativePath;
    }

    public async Task DeleteImageAsync(int id, int editedByCustomerId, CancellationToken ct)
    {
        Customer editor = await EnsureEditorIsEmployeeAsync(editedByCustomerId, ct);
        Product? product = await productRepository.GetByIdWithoutLogsAsync(id, ct);

        if (product is null)
        {
            throw new KeyNotFoundException(string.Format(ErrorMessages.NotFoundFormat, "Product",  $"id '{id}'"));
        }

        fileStorageService.DeleteFile(product.ImagePath);

        product.ImagePath = null;
        await productRepository.UpdateAsync(product, ct);
        await LogEditAsync(id, editor.Id, ct);
    }

    private async Task<Customer> EnsureEditorIsEmployeeAsync(int customerId, CancellationToken ct)
    {
        Customer? customer = await customerRepository.GetByIdAsync(customerId, ct);

        if (customer is null)
        {
            throw new KeyNotFoundException($"Customer with id '{customerId}' not found");
        }

        if (!customer.IsEmployee)
        {
            throw new InvalidOperationException("Only employees can modify products.");
        }

        return customer;
    }

    private async Task LogEditAsync(int productId, int customerId, CancellationToken ct)
    {
        var log = new ProductEditLog
        {
            ProductId = productId,
            CustomerId = customerId,
            EditTime = DateTime.UtcNow
        };

        await productEditLogRepository.AddAsync(log, ct);
    }
}
