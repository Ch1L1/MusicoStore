using MusicoStore.Domain.DTOs.GiftCard;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IGiftCardService
{
    Task<List<GiftCardDTO>> FindAllAsync(CancellationToken ct);
    Task<GiftCardDTO> CreateAsync(CreateGiftCardDTO dto, CancellationToken ct);
    Task ApplyAsync(GiftCardCoupon? coupon, int orderId, CancellationToken ct);
    Task<GiftCardDTO> GetByIdAsync(int giftCardId, CancellationToken ct);
    Task DeleteAsync(int giftCardId, CancellationToken ct);
    Task<GiftCardCouponDTO> GenerateCouponAsync(int giftCardId, CancellationToken ct);
}
