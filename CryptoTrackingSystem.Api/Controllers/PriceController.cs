using CryptoTrackingSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrackingSystem.Api.Controllers;

[ApiController]
[Route("api/{symbol}")]
public class PriceController : ControllerBase
{
    private readonly IPriceService _priceService;

    public PriceController(IPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("24hAvgPrice")]
    public async Task<IActionResult> Get24hAvgPrice(string symbol)
    {
        var result = await _priceService.Get24hAveragePriceAsync(symbol);

        if (result is null)
            return NotFound($"No price data found for {symbol.ToUpper()}.");

        return Ok(result);
    }
}
