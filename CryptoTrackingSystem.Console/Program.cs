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

while (true)
{
    var input = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(input)) continue;
    if (input == "exit") break;

    var parts = input.Split(' ', 2);

    if (parts[0] == "24h" && parts.Length == 2)
    {
        var result = await priceService.Get24hAveragePriceAsync(parts[1]);

        if (result is null)
            Console.WriteLine($"No data found for {parts[1].ToUpper()}.");
        else
            Console.WriteLine($"{result.Symbol} 24h avg: {result.AveragePrice:F2} (from {result.From:u} to {result.To:u})");
    }
    else
    {
        Console.WriteLine("Unknown command for {{symbol}} 24h Usage");
    }
}
