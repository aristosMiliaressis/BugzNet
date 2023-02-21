using System;
using System.Globalization;

namespace BugzNet.Core.Extensions
{
    public static class DateAndTimeExtensions
    {
        public static TimeSpan ToTimeSpan(this string time)
        {
            var timeParts = time.Split(':');

            return new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
        }

        public static string ToDateTimeStamp(this DateTime dateTime, bool includeSecs = true)
        {
            var secs = includeSecs ? ":ss" : "";
            return dateTime.ToString($"dd/MM/yyyy HH:mm{secs}");
        }

        public static double GetDaysSinceNineteenHundred(this DateTime date)
        {
            var millenium = DateTime.ParseExact("1/1/1900", "d/m/yyyy", CultureInfo.InvariantCulture);
            return (date - millenium).TotalDays;
        }

        public static TimeSpan CreateFromToday(int hour, int min)
        {
            return CreateFromToday($"{hour:00}:{min:00}");
        }

        public static TimeSpan CreateFromToday(string timestring)
        {
            if (timestring.IndexOf(":") == 0)
                throw new FormatException("Invalid format in time string. should be HH:mm");

            var timeParts = timestring.Split(':');

            if (timeParts.Length < 2)
                throw new FormatException("Invalid time string");

            return new TimeSpan(Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]), 0);
        }

        public static TimeSpan AddMinutes(this TimeSpan time, int minutes)
        {
            return time.Add(new TimeSpan(0, minutes, 0));
        }

        public static bool IsTimeStamp(this string timeStamp)
        {
            try
            {
                var timeParts = timeStamp.Split(':');

                var time = new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]), 0);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string ToTimeStamp(this TimeSpan time)
        {
            return time.Hours.ToString("D2") + ":" + time.Minutes.ToString("D2");
        }

        public static DateTime StripDatePart(this DateTime dateTime)
        {
            return new DateTime(dateTime.TimeOfDay.Ticks);
        }
    }
}
