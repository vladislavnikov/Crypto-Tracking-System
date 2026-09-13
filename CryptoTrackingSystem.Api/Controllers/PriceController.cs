using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Helpers;
using CryptoTrackingSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoTrackingSystem.Api.Controllers;

[ApiController]
[Route("api/{symbol}")]
public class PriceController : ControllerBase
{
    private readonly IPriceService _priceService;
    private readonly ISmaService _smaService;

    public PriceController(IPriceService priceService, ISmaService smaService)
    {
        _priceService = priceService;
        _smaService = smaService;
    }

    [HttpGet("24hAvgPrice")]
    public async Task<ActionResult<AveragePriceResponse>> Get24hAvgPrice(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Symbol is required.");

        var result = await _priceService.Get24hAveragePriceAsync(symbol);

        if (result is null)
            return NotFound($"No price data found for {symbol.ToUpper()}.");

        return Ok(result);
    }

    [HttpGet("SimpleMovingAverage")]
    public async Task<ActionResult<SmaResponse>> GetSma(string symbol, int n, string p, DateTime? s)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return BadRequest("Symbol is required.");

        if (n <= 0 || n > 365)
            return BadRequest("n must be between 1 and 365.");

        if (!TimePeriodParser.TryParse(p, out var period))
            return BadRequest($"Invalid period '{p}'. Accepted values: 1m, 5m, 30m, 1d, 1w.");

        var startDate = s ?? DateTime.UtcNow;

        if (startDate > DateTime.UtcNow)
            return BadRequest("Start date cannot be in the future.");

        var result = await _smaService.GetSmaAsync(symbol, n, period, startDate);

        if (result is null)
            return NotFound($"No price data found for {symbol.ToUpper()} in the requested window.");

        return Ok(result);
    }
}
