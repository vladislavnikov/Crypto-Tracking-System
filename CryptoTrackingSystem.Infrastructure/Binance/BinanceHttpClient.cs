using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptoTrackingSystem.Infrastructure.Binance;

public class BinanceHttpClient
{
    private readonly HttpClient _httpClient;

    public BinanceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.binance.com");
    }

    public async Task<decimal?> GetCurrentPriceAsync(string symbol)
    {
        var response = await _httpClient.GetAsync($"/api/v3/ticker/price?symbol={symbol.ToUpper()}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BinanceTickerPrice>(json);

        return result is not null && decimal.TryParse(result.Price, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var price)
            ? price
            : null;
    }

    private sealed class BinanceTickerPrice
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;
    }
}
