using System.Text.Json.Serialization;

namespace CryptoTrackingSystem.Infrastructure.Binance;

public class BinanceStreamMessage
{
    [JsonPropertyName("stream")]
    public string Stream { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public MiniTickerData Data { get; set; } = new();
}
