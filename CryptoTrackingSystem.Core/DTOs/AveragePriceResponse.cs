namespace CryptoTrackingSystem.Core.DTOs;

public class AveragePriceResponse
{
    public string Symbol { get; set; } = string.Empty;
    public decimal AveragePrice { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
