using CryptoTrackingSystem.Core.Helpers;
using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
    })
    .Build();

var priceService = host.Services.GetRequiredService<IPriceService>();
var smaService = host.Services.GetRequiredService<ISmaService>();

Console.WriteLine("Commands:");
Console.WriteLine("  24h {symbol}                          - 24h average price");
Console.WriteLine("  sma {symbol} {n} {p} [{startDate}]   - Simple moving average");
Console.WriteLine("  exit                                  - Quit");

while (true)
{
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input)) continue;
    if (input == "exit") break;

    var parts = input.Split(' ');

    if (parts[0] == "24h" && parts.Length == 2)
    {
        var result = await priceService.Get24hAveragePriceAsync(parts[1]);

        if (result is null)
            Console.WriteLine($"No data found for {parts[1].ToUpper()}.");
        else
            Console.WriteLine($"{result.Symbol} 24h avg: {result.AveragePrice:F2} (from {result.From:u} to {result.To:u})");
    }
    else if (parts[0] == "sma" && parts.Length >= 4)
    {
        var symbol = parts[1];

        if (!int.TryParse(parts[2], out var n))
        {
            Console.WriteLine("Invalid value for n. Must be an integer.");
            continue;
        }

        if (!TimePeriodParser.TryParse(parts[3], out var period))
        {
            Console.WriteLine("Invalid period. Accepted values: 1m, 5m, 30m, 1d, 1w.");
            continue;
        }

        var startDate = DateTime.UtcNow;
        if (parts.Length >= 5 && !DateTime.TryParse(parts[4], out startDate))
        {
            Console.WriteLine("Invalid start date format.");
            continue;
        }

        var result = await smaService.GetSmaAsync(symbol, n, period, startDate);

        if (result is null)
            Console.WriteLine($"No data found for {symbol.ToUpper()} in the requested window.");
        else
            Console.WriteLine($"{result.Symbol} SMA({result.NumberOfDataPoints}x{result.PeriodLabel}): {result.Sma:F2} (from {result.From:u} to {result.To:u})");
    }
    else
    {
        Console.WriteLine("Unknown command. Use: 24h {symbol} | sma {symbol} {n} {p} [{startDate}] | exit");
    }
}
