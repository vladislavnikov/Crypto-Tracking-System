using CryptoTrackingSystem.Core.Enums;

namespace CryptoTrackingSystem.Core.Helpers;

public static class TimePeriodParser
{
    public static bool TryParse(string value, out TimePeriod period)
    {
        period = default;

        switch (value)
        {
            case "1m":
                period = TimePeriod.OneMinute;
                return true;
            case "5m":
                period = TimePeriod.FiveMinutes;
                return true;
            case "30m":
                period = TimePeriod.ThirtyMinutes;
                return true;
            case "1d":
                period = TimePeriod.OneDay;
                return true;
            case "1w":
                period = TimePeriod.OneWeek;
                return true;
            default:
                return false;
        }
    }

    public static TimeSpan ToTimeSpan(TimePeriod period)
    {
        return period switch
        {
            TimePeriod.OneMinute => TimeSpan.FromMinutes(1),
            TimePeriod.FiveMinutes => TimeSpan.FromMinutes(5),
            TimePeriod.ThirtyMinutes => TimeSpan.FromMinutes(30),
            TimePeriod.OneDay => TimeSpan.FromDays(1),
            TimePeriod.OneWeek => TimeSpan.FromDays(7),
            _ => throw new ArgumentOutOfRangeException(nameof(period))
        };
    }

    public static string ToBinanceInterval(TimePeriod period)
    {
        return period switch
        {
            TimePeriod.OneMinute => "1m",
            TimePeriod.FiveMinutes => "5m",
            TimePeriod.ThirtyMinutes => "30m",
            TimePeriod.OneDay => "1d",
            TimePeriod.OneWeek => "1w",
            _ => throw new ArgumentOutOfRangeException(nameof(period))
        };
    }
}
