using System;
using System.Globalization;

namespace ScheduleBot.Parser.Extensions
{
    /// <summary>
    /// Определяет методы-расширения для работы с <see cref="DateTime" />
    /// /
    /// Defines extension methods for working with <see cref="DateTime" />
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Возвращает смещение (целым числом) по неделям между двумя датами
        /// /
        /// Returns the offset between two dates by weeks
        /// </summary>
        /// <param name="relativeDateTime">Конечная дата / End date</param>
        /// <param name="startDateTime">Начальная дата / Start date</param>
        /// <param name="firstDayOfWeek">День недели, который нужно использовать в качестве первого дня недели / First day of week</param>
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
