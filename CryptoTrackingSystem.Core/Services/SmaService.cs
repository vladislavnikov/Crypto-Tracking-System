using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Enums;
using CryptoTrackingSystem.Core.Helpers;
using CryptoTrackingSystem.Core.Interfaces;


namespace CryptoTrackingSystem.Core.Services;

public class SmaService : ISmaService
{
    private readonly IBinanceKlineClient _klineClient;

    public SmaService(IBinanceKlineClient klineClient)
    {
        _klineClient = klineClient;
    }

    public async Task<SmaResponse?> GetSmaAsync(string symbol, int n, TimePeriod period, DateTime startDate)
    {
        var periodSpan = TimePeriodParser.ToTimeSpan(period);
        var bucketAverages = new List<decimal>();
        var upperSymbol = symbol.ToUpper();

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

        return new SmaResponse
        {
            Symbol = upperSymbol,
            Sma = bucketAverages.Average(),
            NumberOfDataPoints = bucketAverages.Count,
            PeriodLabel = period.ToString(),
            From = startDate - totalSpan,
            To = startDate
        };
    }
}
