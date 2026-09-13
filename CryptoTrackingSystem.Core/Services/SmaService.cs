using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Enums;
using CryptoTrackingSystem.Core.Helpers;
using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Core.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CryptoTrackingSystem.Core.Services;

public class SmaService : ISmaService
{
    private readonly IBinanceKlineClient _klineClient;
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _cacheSettings;

    public SmaService(IBinanceKlineClient klineClient, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
    {
        _klineClient = klineClient;
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<SmaResponse?> GetSmaAsync(string symbol, int n, TimePeriod period, DateTime startDate)
    {
        var upperSymbol = symbol.ToUpper();
        var cacheKey = $"sma:{upperSymbol}:{n}:{period}:{startDate:O}";

        if (_cache.TryGetValue(cacheKey, out SmaResponse? cached))
            return cached;

        var periodSpan = TimePeriodParser.ToTimeSpan(period);
        var bucketAverages = new List<decimal>();

        for (int i = 0; i < n; i++)
        {
            var bucketEnd = startDate - (periodSpan * i);
            var bucketStart = bucketEnd - periodSpan;

            var avg = await _klineClient.GetAveragePriceForPeriodAsync(upperSymbol, period, bucketStart, bucketEnd);

            if (avg is not null)
                bucketAverages.Add(avg.Value);
        }

        if (bucketAverages.Count == 0)
            return null;

        var totalSpan = periodSpan * n;

        var result = new SmaResponse
        {
            Symbol = upperSymbol,
            Sma = bucketAverages.Average(),
            NumberOfDataPoints = bucketAverages.Count,
            PeriodLabel = period.ToString(),
            From = startDate - totalSpan,
            To = startDate
        };

        _cache.Set(cacheKey, result, periodSpan * _cacheSettings.SmaMultiplier);

        return result;
    }
}
