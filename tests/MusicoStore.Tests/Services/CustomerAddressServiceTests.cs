using AutoMapper;
using Moq;
using MusicoStore.BusinessLayer.Services;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.DTOs.CustomerAddress;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.BussinessLayer.Tests.Services;

public class CustomerAddressServiceTests
    {
    private readonly Mock<ICustomerAddressRepository> _customerAddressRepoMock;
    private readonly Mock<IRepository<Customer>> _customerRepoMock;
    private readonly Mock<IRepository<Address>> _addressRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CustomerAddressService _service;

    private const int CustomerId = 1;

    private static CreateAddressDTO GetValidCreateAddressDTO()
        {
        return new()
            {
            StreetName = "Test Street",
            StreetNumber = "123",
            City = "Prague",
            PostalNumber = "10000",
            CountryCode = "CZE"
            };
        }

    public CustomerAddressServiceTests()
        {
        _customerAddressRepoMock = new Mock<ICustomerAddressRepository>();
        _customerRepoMock = new Mock<IRepository<Customer>>();
        _addressRepoMock = new Mock<IRepository<Address>>();
        _mapperMock = new Mock<IMapper>();

        _service = new CustomerAddressService(
            _customerAddressRepoMock.Object,
            _customerRepoMock.Object,
            _addressRepoMock.Object,
            _mapperMock.Object);

        _customerRepoMock
            .Setup(r => r.DoesEntityExist(CustomerId))
            .Returns(true);
        }

    [Fact]
    public async Task AddAddressAsync_Throws_WhenCustomerNotFound()
        {
        // Arrange
        _customerRepoMock
            .Setup(r => r.DoesEntityExist(CustomerId))
            .Returns(false);

        var dto = new UpsertCustomerAddressDTO { NewAddress = GetValidCreateAddressDTO() };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddAddressAsync(CustomerId, dto, CancellationToken.None));
        }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task AddAddressAsync_Throws_WhenAddressOptionsAreInvalid(bool hasExisting,
        bool hasNew)
        {
        // Arrange
        var dto = new UpsertCustomerAddressDTO
            {
            ExistingAddressId = hasExisting ? 1 : null,
            NewAddress = hasNew ? GetValidCreateAddressDTO() : null
            };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddAddressAsync(CustomerId, dto, CancellationToken.None));
        }

    [Fact]
    public async Task AddAddressAsync_Throws_WhenExistingAddressIdNotFound()
        {
        // Arrange
        const int existingId = 99;
        var dto = new UpsertCustomerAddressDTO { ExistingAddressId = existingId };

        _addressRepoMock
            .Setup(r => r.GetByIdAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address)null!);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddAddressAsync(CustomerId, dto, CancellationToken.None));
        }

    [Fact]
    public async Task AddAddressAsync_UsesExistingAddress_AndCreatesNewCustomerAddress()
        {
        // Arrange
        const int existingAddressId = 10;
        var dto = new UpsertCustomerAddressDTO { ExistingAddressId = existingAddressId, IsMainAddress = false };
        var addressEntity = new Address { Id = existingAddressId };
        var newCustomerAddressEntity = new CustomerAddress
            { Id = 1, CustomerId = CustomerId, AddressId = existingAddressId };
        var mappedDto = new CustomerAddressSummaryForCustomerDTO();

        _addressRepoMock
            .Setup(r => r.GetByIdAsync(existingAddressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressEntity);

        _customerAddressRepoMock
            .Setup(r => r.GetByCustomerAndAddressAsync(CustomerId, existingAddressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddress)null!);

        _customerAddressRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CustomerAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newCustomerAddressEntity);

        _mapperMock
            .Setup(m => m.Map<CustomerAddressSummaryForCustomerDTO>(newCustomerAddressEntity))
            .Returns(mappedDto);

        // Act
        CustomerAddressSummaryForCustomerDTO result =
            await _service.AddAddressAsync(CustomerId, dto, CancellationToken.None);

        // Assert
        Assert.Equal(mappedDto, result);
        _addressRepoMock.Verify(r => r.GetByIdAsync(existingAddressId, It.IsAny<CancellationToken>()), Times.Once);
        _customerAddressRepoMock.Verify(r => r.AddAsync(
            It.Is<CustomerAddress>(ca => ca.AddressId == existingAddressId),
            It.IsAny<CancellationToken>()), Times.Once);
        _customerAddressRepoMock.Verify(
            r => r.UnsetAllMainAddressesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    [Fact]
    public async Task AddAddressAsync_UsesExistingAddress_WhenDuplicateFoundForNewAddress()
        {
        // Arrange
        var newAddressDto = GetValidCreateAddressDTO();
        var duplicateAddress = new Address { Id = 20 };
        var dto = new UpsertCustomerAddressDTO { NewAddress = newAddressDto, IsMainAddress = false };
        var createdCustomerAddress = new CustomerAddress { Id = 2, CustomerId = CustomerId, AddressId = 20 };
        var mappedDto = new CustomerAddressSummaryForCustomerDTO();

        _customerAddressRepoMock
            .Setup(r => r.FindDuplicateAddressAsync(newAddressDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(duplicateAddress);

        _customerAddressRepoMock
            .Setup(r => r.GetByCustomerAndAddressAsync(CustomerId, duplicateAddress.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddress)null!);

        _customerAddressRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CustomerAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCustomerAddress);

        _mapperMock
            .Setup(m => m.Map<CustomerAddressSummaryForCustomerDTO>(createdCustomerAddress))
            .Returns(mappedDto);

        // Act
        CustomerAddressSummaryForCustomerDTO result =
            await _service.AddAddressAsync(CustomerId, dto, CancellationToken.None);

        // Assert
        Assert.Equal(mappedDto, result);
        _customerAddressRepoMock.Verify(r => r.FindDuplicateAddressAsync(newAddressDto, It.IsAny<CancellationToken>()),
            Times.Once);
        _addressRepoMock.Verify(r => r.AddAsync(It.IsAny<Address>(), It.IsAny<CancellationToken>()), Times.Never);
        }

    [Fact]
    public async Task AddAddressAsync_CreatesNewAddress_WhenNoDuplicateFound()
        {
        // Arrange
        var newAddressDto = GetValidCreateAddressDTO();
        var newAddressEntity = new Address { Id = 30 };
        var dto = new UpsertCustomerAddressDTO { NewAddress = newAddressDto, IsMainAddress = false };
        var mappedAddress = new Address();
        var createdCustomerAddress = new CustomerAddress { Id = 3, CustomerId = CustomerId, AddressId = 30 };
        var mappedDto = new CustomerAddressSummaryForCustomerDTO();

        _customerAddressRepoMock
            .Setup(r => r.FindDuplicateAddressAsync(newAddressDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address)null!);

        _mapperMock
            .Setup(m => m.Map<Address>(newAddressDto))
            .Returns(mappedAddress);

        _addressRepoMock
            .Setup(r => r.AddAsync(mappedAddress, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAddressEntity);

        _customerAddressRepoMock
            .Setup(r => r.GetByCustomerAndAddressAsync(CustomerId, 30, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddress)null!);

        _customerAddressRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CustomerAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCustomerAddress);

        _mapperMock
            .Setup(m => m.Map<CustomerAddressSummaryForCustomerDTO>(createdCustomerAddress))
            .Returns(mappedDto);

        // Act
        CustomerAddressSummaryForCustomerDTO result =
            await _service.AddAddressAsync(CustomerId, dto, CancellationToken.None);

        // Assert
        Assert.Equal(mappedDto, result);
        _addressRepoMock.Verify(r => r.AddAsync(mappedAddress, It.IsAny<CancellationToken>()), Times.Once);
        }

    [Fact]
    public async Task AddAddressAsync_UnsetsMainAddressAndUpdatesExistingCustomerAddress_WhenSettingAsMain()
        {
        // Arrange
        const int addressId = 40;
        var dto = new UpsertCustomerAddressDTO { ExistingAddressId = addressId, IsMainAddress = true };
        var addressEntity = new Address { Id = addressId };

        var existingCustomerAddress = new CustomerAddress
            {
            Id = 4,
            CustomerId = CustomerId,
            AddressId = addressId,
            IsMainAddress = false
            };
        var mappedDto = new CustomerAddressSummaryForCustomerDTO();

        _addressRepoMock
            .Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressEntity);

        _customerAddressRepoMock
            .Setup(r => r.GetByCustomerAndAddressAsync(CustomerId, addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomerAddress);

        _mapperMock
            .Setup(m => m.Map<CustomerAddressSummaryForCustomerDTO>(existingCustomerAddress))
            .Returns(mappedDto);

        // Act
        var result = await _service.AddAddressAsync(CustomerId, dto, CancellationToken.None);

        // Assert
        _customerAddressRepoMock.Verify(r => r.UnsetAllMainAddressesAsync(CustomerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _customerAddressRepoMock.Verify(r => r.UpdateAsync(existingCustomerAddress, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.True(existingCustomerAddress.IsMainAddress);
        Assert.Equal(mappedDto, result);
        }

    [Fact]
    public async Task AddAddressAsync_UnsetsMainAddressAndCreatesNewCustomerAddress_WhenSettingNewAsMain()
        {
        // Arrange
        const int addressId = 50;
        var dto = new UpsertCustomerAddressDTO { ExistingAddressId = addressId, IsMainAddress = true };
        var addressEntity = new Address { Id = addressId };
        var createdCustomerAddress = new CustomerAddress { Id = 5, CustomerId = CustomerId, AddressId = addressId };
        var mappedDto = new CustomerAddressSummaryForCustomerDTO();

        _addressRepoMock
            .Setup(r => r.GetByIdAsync(addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressEntity);

        _customerAddressRepoMock
            .Setup(r => r.GetByCustomerAndAddressAsync(CustomerId, addressId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerAddress)null!);

        _customerAddressRepoMock
            .Setup(r => r.AddAsync(It.IsAny<CustomerAddress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCustomerAddress);

        _mapperMock
            .Setup(m => m.Map<CustomerAddressSummaryForCustomerDTO>(createdCustomerAddress))
            .Returns(mappedDto);

        // Act
        CustomerAddressSummaryForCustomerDTO result =
            await _service.AddAddressAsync(CustomerId, dto, CancellationToken.None);

        // Assert
        _customerAddressRepoMock.Verify(r => r.UnsetAllMainAddressesAsync(CustomerId, It.IsAny<CancellationToken>()),
            Times.Once);

        _customerAddressRepoMock.Verify(r => r.AddAsync(
            It.Is<CustomerAddress>(ca => ca.IsMainAddress && ca.AddressId == addressId),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(mappedDto, result);
        }
    }
