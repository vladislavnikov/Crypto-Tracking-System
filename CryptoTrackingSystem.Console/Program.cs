using System.Globalization;
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

await host.StartAsync();

PrintHelp();

while (true)
{
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input)) continue;
    if (input == "exit")
    {
        await host.StopAsync();
        break;
    }

    var parts = input.Split(' ');

    switch (parts[0])
    {
        case "24h" when parts.Length == 2:
            await Handle24hAsync(parts[1]);
            break;
        case "sma" when parts.Length >= 4:
            await HandleSmaAsync(parts);
            break;
        default:
            Console.WriteLine("Unknown command. Use: 24h {symbol} | sma {symbol} {n} {p} [{startDate}] | exit");
            break;
    }
}

void PrintHelp()
{
    Console.WriteLine("Commands:");
    Console.WriteLine("  24h {symbol} - 24h average price");
    Console.WriteLine("  sma {symbol} {n} {p} [{startDate}] - Simple moving average");
    Console.WriteLine("  exit - Quit");
}

async Task Handle24hAsync(string symbol)
{
    var result = await priceService.Get24hAveragePriceAsync(symbol);

    if (result is null) { 
        Console.WriteLine($"No data found for {symbol.ToUpper()}.");
    }
    else { 
        Console.WriteLine($"{result.Symbol} 24h avg: {result.AveragePrice:F2} (from {result.From:u} to {result.To:u})");
    }
}

async Task HandleSmaAsync(string[] parts)
{
    var symbol = parts[1];

    if (!int.TryParse(parts[2], out var n))
    {
        Console.WriteLine("Invalid value for n. Must be an integer.");
        return;
    }

    if (n <= 0 || n > 365)
    {
        Console.WriteLine("n must be between 1 and 365.");
        return;
    }

    if (!TimePeriodParser.TryParse(parts[3], out var period))
    {
        Console.WriteLine("Invalid period. Accepted values: 1m, 5m, 30m, 1d, 1w.");
        return;
    }

    var startDate = DateTime.UtcNow;
    if (parts.Length >= 5)
    {
        if (!DateTime.TryParse(parts[4], null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out startDate))
        {
            Console.WriteLine("Invalid start date format.");
            return;
        }

        if (startDate > DateTime.UtcNow)
        {
            Console.WriteLine("Start date cannot be in the future.");
            return;
        }
    }

    var result = await smaService.GetSmaAsync(symbol, n, period, startDate);

    if (result is null) { 
        Console.WriteLine($"No data found for {symbol.ToUpper()} in the requested window.");
    }
    else { 
        Console.WriteLine($"{result.Symbol} SMA({result.NumberOfDataPoints}x{result.PeriodLabel}): {result.Sma:F2} (from {result.From:u} to {result.To:u})");
    }
}
