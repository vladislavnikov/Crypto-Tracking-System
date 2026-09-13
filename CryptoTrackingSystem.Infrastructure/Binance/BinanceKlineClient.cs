using System.Text.Json;
using CryptoTrackingSystem.Core.Enums;
using CryptoTrackingSystem.Core.Helpers;
using CryptoTrackingSystem.Core.Interfaces;

namespace CryptoTrackingSystem.Infrastructure.Binance;

public class BinanceKlineClient : IBinanceKlineClient
{
    private const string KlinesBaseUrl = "https://api.binance.com/api/v3/klines";
    private static string BuildKlinesUrl(string symbol, string interval, long startMs, long endMs, int limit = 1)
    => $"{KlinesBaseUrl}?symbol={symbol}&interval={interval}&startTime={startMs}&endTime={endMs}&limit={limit}";

    private readonly HttpClient _httpClient;

    public BinanceKlineClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal?> GetAveragePriceForPeriodAsync(string symbol, TimePeriod period, DateTime from, DateTime to)
    {
        var interval = TimePeriodParser.ToBinanceInterval(period);
        var startMs = new DateTimeOffset(from, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var endMs = new DateTimeOffset(to, TimeSpan.Zero).ToUnixTimeMilliseconds();

        var url = BuildKlinesUrl(symbol, interval, startMs, endMs);

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var klines = JsonSerializer.Deserialize<JsonElement[][]>(json);

        if (klines is null || klines.Length == 0)
            return null;

        var closePrice = klines[0][4].GetString();

        if (decimal.TryParse(closePrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var price))
            return price;

        return null;
    }
}
