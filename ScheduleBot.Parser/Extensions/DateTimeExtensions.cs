using System;
using System.Globalization;

namespace ScheduleBot.Parser.Extensions
{
    public static class DateTimeExtensions
    {
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
