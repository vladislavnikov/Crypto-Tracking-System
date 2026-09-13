using CryptoTrackingSystem.Core.Enums;
using CryptoTrackingSystem.Core.Interfaces;
using CryptoTrackingSystem.Core.Services;
using CryptoTrackingSystem.Core.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace CryptoTrackingSystem.Tests;

public class SmaServiceTests
{
    private Mock<IBinanceKlineClient> _klineClientMock;
    private IMemoryCache _cache;
    private IOptions<CacheSettings> _cacheSettings;
    private SmaService _service;

    private static readonly DateTime StartDate = new(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc);

    [SetUp]
    public void SetUp()
    {
        _klineClientMock = new Mock<IBinanceKlineClient>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _cacheSettings = Options.Create(new CacheSettings { SmaMultiplier = 1.0 });
        _service = new SmaService(_klineClientMock.Object, _cache, _cacheSettings);
    }

    [TearDown]
    public void TearDown() => _cache.Dispose();

    [Test]
    public async Task GetSmaAsync_ReturnsCorrectAverage_WhenAllBucketsHaveData()
    {
        _klineClientMock
            .Setup(k => k.GetAveragePriceForPeriodAsync("BTCUSDT", TimePeriod.OneDay, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(76000m);

        var result = await _service.GetSmaAsync("BTCUSDT", 3, TimePeriod.OneDay, StartDate);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Symbol, Is.EqualTo("BTCUSDT"));
        Assert.That(result.Sma, Is.EqualTo(76000m));
        Assert.That(result.NumberOfDataPoints, Is.EqualTo(3));
        Assert.That(result.PeriodLabel, Is.EqualTo("OneDay"));
    }

    [Test]
    public async Task GetSmaAsync_AveragesOnlyNonNullBuckets()
    {
        var callCount = 0;
        _klineClientMock
            .Setup(k => k.GetAveragePriceForPeriodAsync("BTCUSDT", TimePeriod.OneDay, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(() => ++callCount == 2 ? (decimal?)null : 76000m);

        var result = await _service.GetSmaAsync("BTCUSDT", 3, TimePeriod.OneDay, StartDate);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.NumberOfDataPoints, Is.EqualTo(2));
        Assert.That(result.Sma, Is.EqualTo(76000m));
    }

    [Test]
    public async Task GetSmaAsync_ReturnsNull_WhenAllBucketsEmpty()
    {
        _klineClientMock
            .Setup(k => k.GetAveragePriceForPeriodAsync("FAKEUSDT", It.IsAny<TimePeriod>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((decimal?)null);

        var result = await _service.GetSmaAsync("FAKEUSDT", 3, TimePeriod.OneDay, StartDate);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetSmaAsync_ReturnsCachedResult_OnSecondCall()
    {
        _klineClientMock
            .Setup(k => k.GetAveragePriceForPeriodAsync("BTCUSDT", TimePeriod.OneDay, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(76000m);

        await _service.GetSmaAsync("BTCUSDT", 3, TimePeriod.OneDay, StartDate);
        await _service.GetSmaAsync("BTCUSDT", 3, TimePeriod.OneDay, StartDate);

        _klineClientMock.Verify(
            k => k.GetAveragePriceForPeriodAsync(It.IsAny<string>(), It.IsAny<TimePeriod>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Exactly(3)); // 3 buckets, called once total (second call hits cache)
    }

    [Test]
    public async Task GetSmaAsync_SetsCorrectFromAndTo()
    {
        _klineClientMock
            .Setup(k => k.GetAveragePriceForPeriodAsync("BTCUSDT", TimePeriod.OneDay, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(76000m);

        var result = await _service.GetSmaAsync("BTCUSDT", 5, TimePeriod.OneDay, StartDate);

        Assert.That(result!.To, Is.EqualTo(StartDate));
        Assert.That(result.From, Is.EqualTo(StartDate.AddDays(-5)));
    }
}
