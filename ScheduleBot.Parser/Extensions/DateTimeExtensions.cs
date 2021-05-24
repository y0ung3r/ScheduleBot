using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ScheduleBot.Parser.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetStartOfWeek(this DateTime dateTime, DayOfWeek requiredStartOfWeekDay)
        {
            var daysInWeek = 7;
            var difference = (daysInWeek + dateTime.DayOfWeek - requiredStartOfWeekDay) % daysInWeek;

            return dateTime.AddDays(-difference).Date;
        }

        public static ICollection<DateTime> GetWeekDates(this DateTime dateTime, DayOfWeek startOfWeekDay)
        {
            return Enumerable.Range
            (
                start: 0,
                count: 7
            )
            .Select(dayNumber =>
            {
                var startOfWeekDate = dateTime.Date.GetStartOfWeek(startOfWeekDay);
                return startOfWeekDate.AddDays(dayNumber);
            })
            .ToList();
        }

        public static int GetRelativeWeek(this DateTime relativeDateTime, DateTime startDateTime, DayOfWeek firstDayOfWeek)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var weekRule = CalendarWeekRule.FirstFourDayWeek;

            var startDate = startDateTime.Date;
            var startWeekNumber = calendar.GetWeekOfYear(startDate, weekRule, firstDayOfWeek);

            var relativeDate = relativeDateTime.Date;
            var relativeWeekNumber = calendar.GetWeekOfYear(relativeDate, weekRule, firstDayOfWeek);

            return Math.Abs(relativeWeekNumber - startWeekNumber) * relativeDate.CompareTo(startDate);
        }
    }
}
