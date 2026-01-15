using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.BusinessLayer.Services;

public interface ICurrencyConversionService
{
    decimal Convert(decimal amount, Currency fromCurrency, Currency toCurrency);
}
