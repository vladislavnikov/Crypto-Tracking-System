using System.ComponentModel.DataAnnotations;

namespace CryptoTrackingSystem.Infrastructure.Data.Models;

public class PriceRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}