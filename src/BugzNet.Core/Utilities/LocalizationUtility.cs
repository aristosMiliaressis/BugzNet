using Microsoft.Extensions.Logging;
using BugzNet.Core.Utilities;
using System;
using System.Globalization;
using TimeZoneConverter;

namespace BugzNet.Core.Localization
{
    public class LocalizationUtility
    {
        private static ILogger _logger => LoggingUtility.LoggerFactory.CreateLogger(nameof(LocalizationUtility));
        public static TimeZoneInfo TimeZone = TZConvert.GetTimeZoneInfo("Europe/Athens");

        public static void ConfigureTimeZone(string timeZone)
        {
            if (timeZone == null || !TZConvert.TryGetTimeZoneInfo(timeZone, out TimeZoneInfo tz))
            {
                _logger.LogWarning("Failed to parse timezone {userTimeZone} defaulting to {defaultTimeZone}", timeZone, TimeZone);
                tz = TimeZone;
            }

            TimeZone = tz;

            // Explicitly set thread culture to prevent validation errors
            // caused by implicit datetime format validation based on thread culture
            var cultureInfo = new CultureInfo("el");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        private static TimeSpan OffsetFromUtc(DateTime time)
        {
            var offset = TimeZone.GetUtcOffset(time);

            return offset;
        }

        public static DateTimeOffset AddTimeZoneInfo(DateTime value)
        {
            if (value.Year == 0 || value.Year == 1)
                return DateTimeOffset.MinValue;

            value = new DateTime(value.Ticks, DateTimeKind.Unspecified);

            var offset = new DateTimeOffset(value, OffsetFromUtc(value));

            return offset;
        }

        public static DateTime LocalToUtc(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(new DateTime(datetime.Ticks, DateTimeKind.Unspecified), TimeZone);
        }

        public static DateTime UtcToLocal(DateTime datetime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(datetime.Ticks, DateTimeKind.Unspecified), TimeZone);
        }

        public static DateTime LocalTime
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
            }
        }

        public static DateTime DateTimeOffsetToLocalTime(DateTimeOffset v)
        {
            return DateTime.SpecifyKind(v.DateTime, DateTimeKind.Local);
        }
    }
}
