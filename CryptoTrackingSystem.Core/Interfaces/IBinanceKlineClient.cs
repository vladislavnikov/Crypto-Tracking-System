using CryptoTrackingSystem.Core.Enums;

namespace CryptoTrackingSystem.Core.Interfaces;

public interface IBinanceKlineClient
{
    Task<decimal?> GetAveragePriceForPeriodAsync(string symbol, TimePeriod period, DateTime from, DateTime to);
}
