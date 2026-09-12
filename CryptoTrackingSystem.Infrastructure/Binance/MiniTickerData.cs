using System.Text.Json.Serialization;

namespace CryptoTrackingSystem.Infrastructure.Binance;

public class MiniTickerData
{
    [JsonPropertyName("s")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("c")]
    public string LastPrice { get; set; } = string.Empty;

    [JsonPropertyName("E")]
    public long EventTime { get; set; }

    public DateTime EventTimeUtc => DateTimeOffset.FromUnixTimeMilliseconds(EventTime).UtcDateTime;
}
