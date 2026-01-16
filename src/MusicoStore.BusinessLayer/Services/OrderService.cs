using AutoMapper;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.BusinessLayer.Services;

public class OrderService(
    IRepository<Order> orderRepository,
    IRepository<OrderedProduct> orderedProductRepository,
    IOrderStatusLogRepository orderStatusLogRepository,
    IRepository<OrderState> orderStateRepository,
    IProductRepository productRepository,
    IStockRepository stockRepository,
    ICustomerAddressRepository customerAddressRepository,
    IGiftCardCouponRepository giftCardCouponRepository,
    IGiftCardService giftCardService,
    ICurrencyConversionService currencyConversionService,
    IMapper mapper)
    : IOrderService
{
    public async Task<List<OrderDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Order> orders = await orderRepository.GetAllAsync(ct);
        var result = new List<OrderDTO>();

        foreach (var order in orders)
        {
            result.Add(await BuildOrderDtoAsync(order, ct));
        }

        return result;
    }

    public async Task<OrderDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Order? order = await orderRepository.GetByIdAsync(id, ct);
        if (order is null)
        {
            throw new KeyNotFoundException($"Order with id '{id}' not found");
        }

        return await BuildOrderDtoAsync(order, ct);
    }

    public async Task<List<OrderDTO>> FindByCustomerIdAsync(int customerId, CancellationToken ct)
    {
        IReadOnlyList<Order> orders = await orderRepository.GetAllAsync(ct);
        var filtered = orders.Where(o => o.CustomerId == customerId).ToList();

        var result = new List<OrderDTO>();
        foreach (var order in filtered)
        {
            result.Add(await BuildOrderDtoAsync(order, ct));
        }

        return result;
    }

    public bool DoesExistById(int id)
        => orderRepository.DoesEntityExist(id);

    public async Task<OrderDTO> CreateAsync(CreateOrderDTO dto, CancellationToken ct)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            throw new ArgumentException("An order must contain at least one item.");
        }

        if (!dto.CustomerAddressId.HasValue)
        {
            var mainAddress = await customerAddressRepository
                .GetMainAddressForCustomerAsync(dto.CustomerId, ct);

            if (mainAddress == null)
            {
                throw new InvalidOperationException(
                    $"Customer {dto.CustomerId} does not have any main address.");
            }

            dto.CustomerAddressId = mainAddress.AddressId;
        }

        if (dto.CustomerAddressId.HasValue)
        {
            var assigned = await customerAddressRepository
                .GetByCustomerAndAddressAsync(dto.CustomerId, dto.CustomerAddressId.Value, ct);

            if (assigned == null)
            {
                throw new InvalidOperationException(
                    $"Customer {dto.CustomerId} does not have address {dto.CustomerAddressId} assigned.");
            }
        }

        Order order = mapper.Map<Order>(dto);
        Order created = await orderRepository.AddAsync(order, ct);

        foreach (var item in dto.Items)
        {
            Product? product = await productRepository.GetByIdAsync(item.ProductId, ct)
                ?? throw new KeyNotFoundException($"Product {item.ProductId} not found");

            OrderedProduct op = new()
            {
                OrderId = created.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                PricePerItem = product.CurrentPrice
            };

            await orderedProductRepository.AddAsync(op, ct);

            IReadOnlyList<Stock> stocks = await stockRepository.GetAllAsync(ct);
            Stock? stock = stocks.FirstOrDefault(s => s.ProductId == product.Id);

            if (stock != null)
            {
                stock.CurrentQuantity -= item.Quantity;
                await stockRepository.UpdateAsync(stock, ct);
            }
        }

        IReadOnlyList<OrderState> states = await orderStateRepository.GetAllAsync(ct);
        OrderState? createdState = states.FirstOrDefault(s => s.Name == "Created");

        if (createdState is null)
        {
            throw new InvalidOperationException("OrderState 'Created' not found.");
        }

        OrderStatusLog log = new()
        {
            OrderId = created.Id,
            OrderStateId = createdState.Id,
            LogTime = DateTime.UtcNow
        };

        await orderStatusLogRepository.AddAsync(log, ct);

        return await BuildOrderDtoAsync(created, ct);
    }

    public async Task ChangeStateAsync(ChangeOrderStateDTO dto, CancellationToken ct)
    {
        if (!orderRepository.DoesEntityExist(dto.OrderId))
        {
            throw new KeyNotFoundException($"Order {dto.OrderId} not found");
        }

        OrderState? state = await orderStateRepository.GetByIdAsync(dto.NewStateId, ct);
        if (state is null)
        {
            throw new KeyNotFoundException($"OrderState {dto.NewStateId} not found");
        }

        OrderStatusLog log = new()
        {
            OrderId = dto.OrderId,
            OrderStateId = state.Id,
            LogTime = DateTime.UtcNow
        };

        await orderStatusLogRepository.AddAsync(log, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await orderRepository.DeleteAsync(id, ct);

    private async Task<OrderDTO> BuildOrderDtoAsync(Order order, CancellationToken ct)
    {
        OrderDTO dto = mapper.Map<OrderDTO>(order);
        dto.OrderId = order.Id;
        dto.CustomerId = order.CustomerId;
        dto.CustomerAddressId = order.CustomerAddressId;

        var items = (order.OrderedProducts ?? new List<OrderedProduct>())
            .Select(op => new OrderItemDTO
            {
                ProductId = op.ProductId,
                ProductName = op.Product?.Name ?? string.Empty,
                Quantity = op.Quantity,
                PricePerItem = op.PricePerItem,
                LineTotal = op.PricePerItem * op.Quantity
            })
            .ToList();

        dto.Items = items;
        var total = items.Sum(i => i.LineTotal);


        if (order.OrderedProducts == null || !order.OrderedProducts.Any())
        {
            throw new InvalidOperationException("Order has no items.");
        }

        var currencies = new HashSet<Currency>();

        foreach (var op in order.OrderedProducts)
        {
            if (op.Product == null)
            {
                var product = await productRepository.GetByIdAsync(op.ProductId, ct);

                if (product == null)
                {
                    throw new InvalidOperationException($"Product {op.ProductId} not found.");
                }

                currencies.Add(product.CurrencyCode);
            }
            else
            {
                currencies.Add(op.Product.CurrencyCode);
            }
        }

        Console.WriteLine($"Distinct currencies count: {currencies.Count}");

        if (currencies.Count == 0)
        {
            throw new InvalidOperationException("Order has no items with a defined currency.");
        }

        if (currencies.Count > 1)
        {
            throw new InvalidOperationException(
                "Order contains items with multiple currencies. A single currency is expected.");
        }

        var targetCurrency = currencies.First();

        if (order.GiftCardCoupon != null)
        {
            var gift = order.GiftCardCoupon.GiftCard;

            var convertedGiftAmount =
                currencyConversionService.Convert(
                    gift.Amount,
                    gift.CurrencyCode,
                    targetCurrency);

            total -= convertedGiftAmount;
            total = Math.Max(total, 0);
        }

        dto.TotalAmount = total;

        if (order.GiftCardCoupon?.GiftCard != null)
        {
            dto.GiftCardCouponCode = order.GiftCardCoupon.CouponCode;
            var gift = order.GiftCardCoupon.GiftCard;
            var convertedGiftAmount = currencyConversionService.Convert(gift.Amount, gift.CurrencyCode, targetCurrency);
            dto.GiftCardAmount = convertedGiftAmount;
        }

        var logsForOrder = (order.StatusLog ?? new List<OrderStatusLog>())
            .OrderBy(l => l.LogTime)
            .ToList();

        if (logsForOrder.Count > 0)
        {
            dto.CreatedAt = logsForOrder.First().LogTime;
            var latest = logsForOrder.Last();
            dto.CurrentState = latest.OrderState?.Name ?? "Unknown";
        }
        else
        {
            dto.CreatedAt = DateTime.UtcNow;
            dto.CurrentState = "Unknown";
        }

        return dto;
    }

    public async Task ApplyGiftCardAsync(
    int orderId,
    string couponCode,
    CancellationToken ct)
    {
        Order? order = await orderRepository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found");

        if (order.GiftCardCoupon != null)
        {
            throw new InvalidOperationException("A gift card is already applied to this order.");
        }

        IReadOnlyList<GiftCardCoupon> coupons = await giftCardCouponRepository.GetAllAsync(ct);

        GiftCardCoupon? coupon = coupons.FirstOrDefault(c => c.CouponCode == couponCode);

        if (coupon is null)
        {
            throw new KeyNotFoundException("Coupon code does not exist.");
        }

        if (coupon.OrderId.HasValue)
        {
            throw new InvalidOperationException("This coupon has already been used.");
        }

        if (!await IsOrderInCreatedStateAsync(order.Id, ct))
        {
            throw new InvalidOperationException("Gift card can only be applied while the order is in Created state.");
        }

        var now = DateTime.UtcNow;
        if (now < coupon.GiftCard.ValidFrom || now > coupon.GiftCard.ValidTo)
        {
            throw new InvalidOperationException("Gift card is not valid.");
        }

        await giftCardService.ApplyAsync(coupon, order.Id, ct);
        
        order.GiftCardCoupon = coupon;
        await orderRepository.UpdateAsync(order, ct);
    }

    public async Task RemoveGiftCardAsync(int orderId, CancellationToken ct)
    {
        if (!await IsOrderInCreatedStateAsync(orderId, ct))
        {
            throw new InvalidOperationException("Gift card can only be removed while the order is in Created state.");
        }

        GiftCardCoupon? coupon =
            await giftCardCouponRepository.GetByOrderIdAsync(orderId, ct);

        if (coupon == null)
        {
            throw new InvalidOperationException("No gift card is applied to this order.");
        }

        coupon.OrderId = null;

        await giftCardCouponRepository.UpdateAsync(coupon, ct);
    }

    public async Task<List<string>> GetOrderStates(CancellationToken ct)
    {
        IReadOnlyList<OrderState> states = await orderStateRepository.GetAllAsync(ct);
        return states.OrderBy((state => state.Id)).Select((state => state.Name)).ToList();
    }


    private async Task<bool> IsOrderInCreatedStateAsync(
    int orderId,
    CancellationToken ct)
    {
        var stateName = await orderStatusLogRepository.GetLatestStateNameAsync(orderId, ct);

        return stateName == "Created";
    }
}
