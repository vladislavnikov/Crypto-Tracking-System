using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Interfaces;

namespace CryptoTrackingSystem.Core.Services;

public class PriceService : IPriceService
{
    private readonly IUnitOfWork _unitOfWork;

    public PriceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AveragePriceResponse?> Get24hAveragePriceAsync(string symbol)
    {
        var to = DateTime.UtcNow;
        var from = to.AddHours(-24);

        var avg = await _unitOfWork.Prices.GetAveragePriceAsync(symbol.ToUpper(), from, to);

        if (avg is null)
            return null;

        return new AveragePriceResponse
        {
            Symbol = symbol.ToUpper(),
            AveragePrice = avg.Value,
            From = from,
            To = to
        };
    }
}
