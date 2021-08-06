using ScheduleBot.Domain.Extensions;
using ScheduleBot.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScheduleBot.Domain
{
    public class WeekDatesProvider : IWeekDatesProvider
    {
        public IReadOnlyCollection<DateTime> GetWeekDates(DateTime dateTime, double days, DayOfWeek dayOfWeek)
        {
            return new ReadOnlyCollection<DateTime>
            (
                Enumerable.Range
                (
                    start: 0,
                    count: 7
                )
                .Select
                (
                    dayNumber => dateTime.AddDays(days)
                                         .Date
                                         .GetStartOfWeek(dayOfWeek)
                                         .AddDays(dayNumber)
                )
                .SkipLast(count: 1)
                .ToList()
            );
        }
    }
}
