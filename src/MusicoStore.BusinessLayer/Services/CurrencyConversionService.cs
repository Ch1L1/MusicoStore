using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.BusinessLayer.Services;

public class CurrencyConversionService : ICurrencyConversionService
{
    private readonly Dictionary<Currency, decimal> _usdRates = new()
    {
        { Currency.USD, 1.0m },
        { Currency.EUR, 0.92m }, // 1 USD = 0.92 EUR
        { Currency.CZK, 22.8m }  // 1 USD = 22.8 CZK
    };

    public decimal Convert(decimal amount, Currency fromCurrency, Currency toCurrency)
    {
        if (fromCurrency == toCurrency)
        {
            return amount;
        }
        var amountInUsd = amount / _usdRates[fromCurrency];
        var converted = amountInUsd * _usdRates[toCurrency];
        return Math.Round(converted, 2);
    }
}
