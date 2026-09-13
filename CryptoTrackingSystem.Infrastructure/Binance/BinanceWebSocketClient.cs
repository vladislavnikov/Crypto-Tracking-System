using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CryptoTrackingSystem.Infrastructure.Data;
using CryptoTrackingSystem.Infrastructure.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CryptoTrackingSystem.Infrastructure.Binance;

public class BinanceWebSocketClient : BackgroundService
{
    private const string StreamUrl =
        "wss://stream.binance.com/stream?streams=btcusdt@miniTicker/adausdt@miniTicker/ethusdt@miniTicker";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BinanceWebSocketClient> _logger;

    public BinanceWebSocketClient(IServiceScopeFactory scopeFactory, ILogger<BinanceWebSocketClient> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri(StreamUrl), stoppingToken);
        _logger.LogInformation("Connected to Binance stream.");

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var buffer = new byte[4096];

        while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            var ms = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await ws.ReceiveAsync(buffer, stoppingToken);
                ms.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
                break;

            var json = Encoding.UTF8.GetString(ms.ToArray());
            var message = JsonSerializer.Deserialize<BinanceStreamMessage>(json);

            if (message?.Data == null)
                continue;

            var tick = message.Data;

            db.PriceRecords.Add(new PriceRecord
            {
                Symbol = tick.Symbol,
                Price = decimal.Parse(tick.LastPrice, System.Globalization.CultureInfo.InvariantCulture),
                Timestamp = tick.EventTimeUtc
            });

            await db.SaveChangesAsync(stoppingToken);
        }
    }
}
