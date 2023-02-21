using BugzNet.Core.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BugzNet.Core.Extensions
{
    public static class AggregateByExtensions
    {
        public enum AggregateBy
        {
            Hour,
            Day,
            Month,
            Week
        }

        public static string ToAggregateTimeLabel(this AggregateBy aggregateBy, DateTime time)
        {
            var timeFormat = aggregateBy == AggregateBy.Month
                                        ? "MMM yyyy"
                                        : aggregateBy == AggregateBy.Week
                                          ? "dd/MM/yyyy"
                                          : aggregateBy == AggregateBy.Day
                                              ? "dd/MM/yyyy"
                                              : "dd/MM HH:00";

            return time.ToString(timeFormat, new CultureInfo("en-US"));
        }

        public static DateTime NextAggregateInterval(this DateTime date, AggregateBy aggregateBy)
        {
            switch (aggregateBy)
            {
                case AggregateBy.Hour:
                    return date.AddHours(1);
                case AggregateBy.Day:
                    return date.AddDays(1);
                case AggregateBy.Week:
                    return date.AddDays(7);
                case AggregateBy.Month:
                default:
                    return date.AddMonths(1);
            }
        }

        public static DateTime PrevAggregateInterval(this DateTime date, AggregateBy aggregateBy)
        {
            switch (aggregateBy)
            {
                case AggregateBy.Hour:
                    return date.AddHours(-1);
                case AggregateBy.Day:
                    return date.AddDays(-1);
                case AggregateBy.Week:
                    return date.AddDays(-7);
                case AggregateBy.Month:
                default:
                    return date.AddMonths(-1);
            }
        }

        public static DateTime ToStartOfAggregateUnit(this DateTime date, AggregateBy aggregateBy)
        {
            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            switch (aggregateBy)
            {
                case AggregateBy.Day:
                case AggregateBy.Hour:
                    return date;
                case AggregateBy.Week:
                    int daysUntilSOW = date.DayOfWeek == DayOfWeek.Sunday
                                    ? -6
                                    : 1 - (int)date.DayOfWeek;
                    return date.AddDays(daysUntilSOW);
                case AggregateBy.Month:
                    return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                default:
                    throw new NotSupportedException($"Aggregate {aggregateBy} not supported.");
            }
        }

        public static DateTime ToEndOfAggregateUnit(this DateTime date, AggregateBy aggregateBy)
        {
            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            switch (aggregateBy)
            {
                case AggregateBy.Day:
                case AggregateBy.Hour:
                    if (date.Date == LocalizationUtility.LocalTime.Date)
                        return date.AddHours(LocalizationUtility.LocalTime.Hour);
                    else
                        return date.AddHours(24);
                case AggregateBy.Week:
                    int daysUntilEOW = date.DayOfWeek == DayOfWeek.Sunday
                                    ? 1
                                    : 8 - (int)date.DayOfWeek;
                    return date.AddDays(daysUntilEOW-1);
                case AggregateBy.Month:
                    int daysUntilEOM = DateTime.DaysInMonth(date.Year, date.Month) - date.Day + 1;

                    return date.AddDays(daysUntilEOM-1);
                default:
                    throw new NotSupportedException($"Aggregate {aggregateBy} not supported.");
            }
        }

        public static IDictionary<string, T> GetEmptyDictionary<T>(this AggregateBy aggregateBy, DateTime from, DateTime to)
        {
            var dict = new Dictionary<string, T>();

            for (DateTime i = from.Date; i <= to; i = i.NextAggregateInterval(aggregateBy))
            {
                dict.Add(aggregateBy.ToAggregateTimeLabel(i), default);
            }

            return dict;
        }
    }
}
