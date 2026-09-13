using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Core.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CryptoTrackingSystem.Core.Services;

public class PriceService : IPriceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _cacheSettings;

    public PriceService(IUnitOfWork unitOfWork, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<AveragePriceResponse?> Get24hAveragePriceAsync(string symbol)
    {
        var cacheKey = $"24h:{symbol.ToUpper()}";

        if (_cache.TryGetValue(cacheKey, out AveragePriceResponse? cached))
            return cached;

        var to = DateTime.UtcNow;
        var from = to.AddHours(-24);

        var avg = await _unitOfWork.Prices.GetAveragePriceAsync(symbol.ToUpper(), from, to);

        if (avg is null)
            return null;

        var result = new AveragePriceResponse
        {
            Symbol = symbol.ToUpper(),
            AveragePrice = avg.Value,
            From = from,
            To = to
        };

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_cacheSettings.AveragePrice24hMinutes));

        return result;
    }
}
