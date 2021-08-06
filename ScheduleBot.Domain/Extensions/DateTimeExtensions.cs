using System;

namespace ScheduleBot.Domain.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime GetStartOfWeek(this DateTime dateTime, DayOfWeek requiredStartOfWeekDay)
        {
            var daysInWeek = 7;
            var difference = (daysInWeek + dateTime.DayOfWeek - requiredStartOfWeekDay) % daysInWeek;

            return dateTime.AddDays(-difference).Date;
        }
    }
}
