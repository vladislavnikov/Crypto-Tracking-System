namespace CryptoTrackingSystem.Core.Interfaces;

public interface IPriceRepository
{
    Task<decimal?> GetAveragePriceAsync(string symbol, DateTime from, DateTime to);
}
