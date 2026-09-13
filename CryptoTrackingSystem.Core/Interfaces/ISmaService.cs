using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Enums;

namespace CryptoTrackingSystem.Core.Interfaces;

public interface ISmaService
{
    Task<SmaResponse?> GetSmaAsync(string symbol, int n, TimePeriod period, DateTime startDate);
}
