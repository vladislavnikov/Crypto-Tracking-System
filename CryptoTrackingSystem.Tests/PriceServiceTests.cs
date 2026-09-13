using CryptoTrackingSystem.Core.DTOs;
using CryptoTrackingSystem.Core.Enums;
using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Core.Services;
using CryptoTrackingSystem.Core.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace CryptoTrackingSystem.Tests;

public class PriceServiceTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private IMemoryCache _cache;
    private IOptions<CacheSettings> _cacheSettings;
    private PriceService _service;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheSettings = Options.Create(new CacheSettings { AveragePrice24hMinutes = 1 });
        _service = new PriceService(_unitOfWorkMock.Object, _cache, _cacheSettings);
    }

    [TearDown]
    public void TearDown() => _cache.Dispose();

    [Test]
    public async Task Get24hAveragePriceAsync_ReturnsResponse_WhenDataExists()
    {
        _unitOfWorkMock
            .Setup(u => u.Prices.GetAveragePriceAsync("BTCUSDT", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(77000m);

        var result = await _service.Get24hAveragePriceAsync("BTCUSDT");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Symbol, Is.EqualTo("BTCUSDT"));
        Assert.That(result.AveragePrice, Is.EqualTo(77000m));
        Assert.That(result.From, Is.LessThan(result.To));
    }

    [Test]
    public async Task Get24hAveragePriceAsync_ReturnsNull_WhenNoData()
    {
        _unitOfWorkMock
            .Setup(u => u.Prices.GetAveragePriceAsync("FAKEUSDT", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((decimal?)null);

        var result = await _service.Get24hAveragePriceAsync("FAKEUSDT");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Get24hAveragePriceAsync_ReturnsCachedResult_OnSecondCall()
    {
        _unitOfWorkMock
            .Setup(u => u.Prices.GetAveragePriceAsync("BTCUSDT", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(77000m);

        await _service.Get24hAveragePriceAsync("BTCUSDT");
        await _service.Get24hAveragePriceAsync("BTCUSDT");

        _unitOfWorkMock.Verify(
            u => u.Prices.GetAveragePriceAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Once);
    }
}
