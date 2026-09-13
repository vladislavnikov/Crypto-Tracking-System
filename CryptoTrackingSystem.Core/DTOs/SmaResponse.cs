namespace CryptoTrackingSystem.Core.DTOs;

public class SmaResponse
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Sma { get; set; }
    public int NumberOfDataPoints { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
