using MusicoStore.Domain.DTOs.Order;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IOrderService
{
    Task<List<OrderDTO>> FindAllAsync(CancellationToken ct);
    Task<OrderDTO> FindByIdAsync(int id, CancellationToken ct);
    Task<List<OrderDTO>> FindByCustomerIdAsync(int customerId, CancellationToken ct);
    bool DoesExistById(int id);
    Task<OrderDTO> CreateAsync(CreateOrderDTO dto, CancellationToken ct);
    Task ChangeStateAsync(ChangeOrderStateDTO dto, CancellationToken ct);
    Task DeleteByIdAsync(int id, CancellationToken ct);
    Task ApplyGiftCardAsync(int orderId, string couponCode, CancellationToken ct);
    Task RemoveGiftCardAsync(int orderId, CancellationToken ct);
    Task<List<string>> GetOrderStates(CancellationToken ct);

}
