using ScheduleBot.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace ScheduleBot.Domain.Extensions
{
    public static class WeekDatesProviderExtensions
    {
        public static IReadOnlyCollection<DateTime> GetPreviousWeekDates(this IWeekDatesProvider provider, DateTime dateTime)
        {
            return provider.GetWeekDates
            (
                dateTime,
                days: -7,
                DayOfWeek.Monday
            );
        }

        public static IReadOnlyCollection<DateTime> GetCurrentWeekDates(this IWeekDatesProvider provider, DateTime dateTime)
        {
            return provider.GetWeekDates
            (
                dateTime,
                days: 0,
                DayOfWeek.Monday
            );
        }

        public static IReadOnlyCollection<DateTime> GetNextWeekDates(this IWeekDatesProvider provider, DateTime dateTime)
        {
            return provider.GetWeekDates
            (
                dateTime,
                days: 7,
                DayOfWeek.Monday
            );
        }
    }
}
