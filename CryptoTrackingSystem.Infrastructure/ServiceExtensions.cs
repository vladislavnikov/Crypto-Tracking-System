using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Core.Services;
using CryptoTrackingSystem.Core.Settings;
using CryptoTrackingSystem.Infrastructure.Binance;
using CryptoTrackingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoTrackingSystem.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPriceService, PriceService>();
        services.AddScoped<ISmaService, SmaService>();
        services.AddHttpClient<IBinanceKlineClient, BinanceKlineClient>();

        return services;
    }
}
