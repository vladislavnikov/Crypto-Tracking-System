using CryptoTrackingSystem.Core.DTOs;

namespace CryptoTrackingSystem.Core.Interfaces;

public interface IPriceService
{
    Task<AveragePriceResponse?> Get24hAveragePriceAsync(string symbol);
}
