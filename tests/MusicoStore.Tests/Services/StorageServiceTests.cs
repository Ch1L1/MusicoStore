using AutoMapper;
using Moq;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.Domain.DTOs.Stock;
using MusicoStore.Domain.DTOs.Storage;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Records;

namespace MusicoStore.BussinessLayer.Tests.Services;

public class StorageServiceTests
    {
    private readonly Mock<IRepository<Storage>> _storageRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly StorageService _service;

    public StorageServiceTests()
        {
        _storageRepoMock = new Mock<IRepository<Storage>>();
        _stockRepoMock = new Mock<IStockRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new StorageService(
            _storageRepoMock.Object,
            _stockRepoMock.Object,
            _mapperMock.Object);
        }

    [Fact]
    public async Task FindAllAsync_ReturnsMappedStorages()
        {
        // Arrange
        var storages = new List<Storage>
        {
            new() { Id = 1, Name = "Main", Capacity = 100 },
            new() { Id = 2, Name = "Backup", Capacity = 50 }
        };

        var mappedDtos = new List<StorageDTO> { new(), new() };

        _storageRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storages);

        _mapperMock
            .Setup(m => m.Map<List<StorageDTO>>(storages))
            .Returns(mappedDtos);

        // Act
        var result = await _service.FindAllAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        _storageRepoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<List<StorageDTO>>(storages), Times.Once);
        }

    [Fact]
    public async Task FindByIdAsync_ReturnsMappedStorage()
        {
        // Arrange
        var storage = new Storage { Id = 5, Name = "TestStorage", Capacity = 42 };
        var dto = new StorageDTO();

        _storageRepoMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storage);

        _mapperMock
            .Setup(m => m.Map<StorageDTO>(storage))
            .Returns(dto);

        // Act
        var result = await _service.FindByIdAsync(5, CancellationToken.None);

        // Assert
        Assert.Same(dto, result);
        _storageRepoMock.Verify(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<StorageDTO>(storage), Times.Once);
        }

    [Fact]
    public void DoesExistById_DelegatesToRepository()
        {
        // Arrange
        _storageRepoMock
            .Setup(r => r.DoesEntityExist(10))
            .Returns(true);

        // Act
        var exists = _service.DoesExistById(10);

        // Assert
        Assert.True(exists);
        _storageRepoMock.Verify(r => r.DoesEntityExist(10), Times.Once);
        }

    [Fact]
    public async Task CreateAsync_MapsAndSavesStorage()
        {
        // Arrange
        var createDto = new CreateStorageDTO();
        var storageEntity = new Storage();
        var createdEntity = new Storage { Id = 7 };
        var mappedDto = new StorageDTO();

        _mapperMock
            .Setup(m => m.Map<Storage>(createDto))
            .Returns(storageEntity);

        _storageRepoMock
            .Setup(r => r.AddAsync(storageEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);

        _mapperMock
            .Setup(m => m.Map<StorageDTO>(createdEntity))
            .Returns(mappedDto);

        // Act
        var result = await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        Assert.Same(mappedDto, result);
        _mapperMock.Verify(m => m.Map<Storage>(createDto), Times.Once);
        _storageRepoMock.Verify(r => r.AddAsync(storageEntity, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<StorageDTO>(createdEntity), Times.Once);
        }

    [Fact]
    public async Task UpdateAsync_MapsAndCallsUpdate()
        {
        // Arrange
        var updateDto = new UpdateStorageDTO();
        var mappedEntity = new Storage();

        _mapperMock
            .Setup(m => m.Map<Storage>(updateDto))
            .Returns(mappedEntity);

        // Act
        await _service.UpdateAsync(3, updateDto, CancellationToken.None);

        // Assert
        _storageRepoMock.Verify(
            r => r.UpdateAsync(
                It.Is<Storage>(s => s == mappedEntity && s.Id == 3),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }

    [Fact]
    public async Task DeleteByIdAsync_CallsRepositoryDelete()
        {
        // Act
        await _service.DeleteByIdAsync(4, CancellationToken.None);

        // Assert
        _storageRepoMock.Verify(r => r.DeleteAsync(4, It.IsAny<CancellationToken>()), Times.Once);
        }

    [Fact]
    public async Task FindStocksByStorageIdAsync_UsesFilterAndMapping()
        {
        // Arrange
        var stocks = new List<Stock> { new(), new() };
        var mapped = new List<StockSummaryForStorageDTO> { new(), new() };

        _stockRepoMock
            .Setup(r => r.FilterAsync(It.IsAny<StockFilterCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stocks);

        _mapperMock
            .Setup(m => m.Map<List<StockSummaryForStorageDTO>>(stocks))
            .Returns(mapped);

        // Act
        var result = await _service.FindStocksByStorageIdAsync(1, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        _stockRepoMock.Verify(r => r.FilterAsync(It.IsAny<StockFilterCriteria>(), It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<List<StockSummaryForStorageDTO>>(stocks), Times.Once);
        }

    [Fact]
    public async Task AddOrUpdateStockAsync_Throws_WhenCreatingNewWithNegativeQuantity()
        {
        // Arrange
        const int storageId = 1;
        var dto = new StockUpdateDTO
            {
            ProductId = 10,
            QuantityDifference = -1
            };

        _stockRepoMock
            .Setup(r => r.FilterAsync(It.IsAny<StockFilterCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Stock>());

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddOrUpdateStockAsync(storageId, dto, CancellationToken.None));
        }

    [Fact]
    public async Task AddOrUpdateStockAsync_UpdatesExistingStock_WhenValid()
        {
        // Arrange
        const int storageId = 1;
        var dto = new StockUpdateDTO
            {
            ProductId = 10,
            QuantityDifference = 3
            };

        var existing = new Stock
            {
            Id = 5,
            StorageId = storageId,
            ProductId = 10,
            CurrentQuantity = 4
            };

        var mappedDto = new StockSummaryForStorageDTO();

        _stockRepoMock
            .Setup(r => r.FilterAsync(It.IsAny<StockFilterCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Stock> { existing });

        _stockRepoMock
            .Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<StockSummaryForStorageDTO>(existing))
            .Returns(mappedDto);

        // Act
        var result = await _service.AddOrUpdateStockAsync(storageId, dto, CancellationToken.None);

        // Assert
        Assert.Same(mappedDto, result);
        _stockRepoMock.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        _stockRepoMock.Verify(r => r.AddAsync(It.IsAny<Stock>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
