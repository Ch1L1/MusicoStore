using AutoMapper;
using Moq;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.BussinessLayer.Tests.Services;

public class OrderServiceTests
    {
    private readonly Mock<IRepository<Order>> _orderRepoMock = new();
    private readonly Mock<IRepository<OrderedProduct>> _orderedProductRepoMock = new();
    private readonly Mock<IRepository<OrderStatusLog>> _orderStatusLogRepoMock = new();
    private readonly Mock<IRepository<OrderState>> _orderStateRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<IStockRepository> _stockRepoMock = new();
    private readonly Mock<ICustomerAddressRepository> _customerAddressRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly OrderService _service;

    public OrderServiceTests()
        {
        _service = new OrderService(
            _orderRepoMock.Object,
            _orderedProductRepoMock.Object,
            _orderStatusLogRepoMock.Object,
            _orderStateRepoMock.Object,
            _productRepoMock.Object,
            _stockRepoMock.Object,
            _customerAddressRepoMock.Object,
            _mapperMock.Object);
        }

    [Fact]
    public async Task FindByIdAsync_ReturnsMappedOrder_WhenExists_AndNoItemsOrLogs()
        {
        // Arrange
        var order = new Order
            {
            Id = 1,
            CustomerId = 7,
            CustomerAddressId = 20
            };

        _orderRepoMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _orderedProductRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderedProduct>());
        _productRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());
        _orderStatusLogRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderStatusLog>());

        _mapperMock
            .Setup(m => m.Map<OrderDTO>(order))
            .Returns(new OrderDTO());

        // Act
        var dto = await _service.FindByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.Equal(1, dto.OrderId);
        Assert.Equal(7, dto.CustomerId);
        Assert.Equal(20, dto.CustomerAddressId);
        Assert.Equal("Unknown", dto.CurrentState);
        Assert.Equal(0m, dto.TotalAmount);
        Assert.False(dto.Items?.Any() ?? false);

        _orderRepoMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<OrderDTO>(order), Times.Once);
        }

    [Fact]
    public async Task FindByIdAsync_Throws_WhenOrderNotFound()
        {
        // Arrange
        _orderRepoMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.FindByIdAsync(99, CancellationToken.None));
        }

    [Fact]
    public void DoesExistById_DelegatesToRepository()
        {
        // Arrange
        _orderRepoMock
            .Setup(r => r.DoesEntityExist(42))
            .Returns(true);

        // Act
        var exists = _service.DoesExistById(42);

        // Assert
        Assert.True(exists);
        _orderRepoMock.Verify(r => r.DoesEntityExist(42), Times.Once);
        }

    [Fact]
    public async Task CreateAsync_Throws_WhenNoItems()
        {
        // Arrange
        var dto = new CreateOrderDTO
            {
            CustomerId = 1,
            CustomerAddressId = 2,
            Items = new List<CreateOrderItemDTO>()
            };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto, CancellationToken.None));
        }

    [Fact]
    public async Task ChangeStateAsync_Throws_WhenOrderNotFound()
        {
        // Arrange
        var change = new ChangeOrderStateDTO { OrderId = 10, NewStateId = 5 };

        _orderRepoMock
            .Setup(r => r.DoesEntityExist(10))
            .Returns(false);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.ChangeStateAsync(change, CancellationToken.None));
        }

    [Fact]
    public async Task ChangeStateAsync_AddsLog_WhenValid()
        {
        // Arrange
        var change = new ChangeOrderStateDTO { OrderId = 3, NewStateId = 8 };

        _orderRepoMock
            .Setup(r => r.DoesEntityExist(3))
            .Returns(true);

        _orderStateRepoMock
            .Setup(r => r.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderState { Id = 8, Name = "Shipped" });

        _orderStatusLogRepoMock
            .Setup(r => r.AddAsync(It.IsAny<OrderStatusLog>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderStatusLog l, CancellationToken _) => l);

        // Act
        await _service.ChangeStateAsync(change, CancellationToken.None);

        // Assert
        _orderStatusLogRepoMock.Verify(
            r => r.AddAsync(
                It.Is<OrderStatusLog>(l => l.OrderId == 3 && l.OrderStateId == 8),
                It.IsAny<CancellationToken>()),
            Times.Once);
        }
    }
