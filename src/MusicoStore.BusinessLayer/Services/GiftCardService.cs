using Microsoft.Extensions.Caching.Memory;
using MusicoStore.Domain.DTOs.GiftCard;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class GiftCardService(
    IRepository<GiftCard> giftCardRepository,
    IGiftCardCouponRepository giftCardCouponRepository,
    IMemoryCache cache)
    : IGiftCardService
{
    private static readonly MemoryCacheEntryOptions CacheOptions =
        new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

    private static string GiftCardCacheKey(int id) => $"giftcard_{id}";

    public async Task<List<GiftCardDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<GiftCard> giftCards = await giftCardRepository.GetAllAsync(ct);

        return giftCards.Select(g => new GiftCardDTO
        {
            Id = g.Id,
            Amount = g.Amount,
            CurrencyCode = g.CurrencyCode.ToString(),
            ValidFrom = g.ValidFrom,
            ValidTo = g.ValidTo,
            Coupons = g.Coupons.Select(c => new GiftCardCouponDTO
            {
                Id = c.Id,
                CouponCode = c.CouponCode,
                OrderId = c.OrderId
            }).ToList()
        }).ToList();
    }

    public async Task<GiftCardDTO> CreateAsync(CreateGiftCardDTO dto, CancellationToken ct)
    {
        if (dto.ValidFrom >= dto.ValidTo)
        {
            throw new ArgumentException("ValidFrom must be before ValidTo.");
        }

        GiftCard giftCard = new()
        {
            Amount = dto.Amount,
            CurrencyCode = Enum.Parse<DataAccessLayer.Enums.Currency>(dto.CurrencyCode),
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo
        };

        GiftCard created = await giftCardRepository.AddAsync(giftCard, ct);

        cache.Remove(GiftCardCacheKey(created.Id));

        return await GetByIdAsync(created.Id, ct);
    }

    public async Task ApplyAsync(GiftCardCoupon? coupon, int orderId, CancellationToken ct)
    {
        if (coupon != null)
        {
            coupon.OrderId = orderId;
            await giftCardCouponRepository.UpdateAsync(coupon, ct);
            cache.Remove(GiftCardCacheKey(coupon.GiftCardId));
        }
    }

    public async Task<GiftCardDTO> GetByIdAsync(int giftCardId, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            GiftCardCacheKey(giftCardId),
            async entry =>
            {
                entry.SetOptions(CacheOptions);

                GiftCard? giftCard = await giftCardRepository.GetByIdAsync(giftCardId, ct);
                if (giftCard is null)
                {
                    throw new KeyNotFoundException($"GiftCard {giftCardId} not found");
                }

                IReadOnlyList<GiftCardCoupon> coupons =
                    await giftCardCouponRepository.GetAllAsync(ct);

                var cardCoupons = coupons
                    .Where(c => c.GiftCardId == giftCard.Id)
                    .Select(c => new GiftCardCouponDTO
                    {
                        Id = c.Id,
                        CouponCode = c.CouponCode,
                        OrderId = c.OrderId
                    })
                    .ToList();

                return new GiftCardDTO
                {
                    Id = giftCard.Id,
                    Amount = giftCard.Amount,
                    CurrencyCode = giftCard.CurrencyCode.ToString(),
                    ValidFrom = giftCard.ValidFrom,
                    ValidTo = giftCard.ValidTo,
                    Coupons = cardCoupons
                };
            })!;
    }

    public async Task DeleteAsync(int giftCardId, CancellationToken ct)
    {
        GiftCard? giftCard = await giftCardRepository.GetByIdAsync(giftCardId, ct);
        if (giftCard is null)
        {
            throw new KeyNotFoundException($"GiftCard {giftCardId} not found");
        }

        await giftCardRepository.DeleteAsync(giftCardId, ct);
        cache.Remove(GiftCardCacheKey(giftCardId));
    }

    public async Task<GiftCardCouponDTO> GenerateCouponAsync(
        int giftCardId,
        CancellationToken ct)
    {
        GiftCard? giftCard = await giftCardRepository.GetByIdAsync(giftCardId, ct);
        if (giftCard is null)
        {
            throw new KeyNotFoundException($"GiftCard {giftCardId} not found");
        }

        GiftCardCoupon coupon = new()
        {
            GiftCardId = giftCard.Id,
            CouponCode = GenerateCode()
        };

        GiftCardCoupon created =
            await giftCardCouponRepository.AddAsync(coupon, ct);

        cache.Remove(GiftCardCacheKey(giftCardId));

        return new GiftCardCouponDTO
        {
            Id = created.Id,
            CouponCode = created.CouponCode,
            OrderId = null
        };
    }

    private static string GenerateCode()
        => Guid.NewGuid().ToString("N")[..10].ToUpper();
}
