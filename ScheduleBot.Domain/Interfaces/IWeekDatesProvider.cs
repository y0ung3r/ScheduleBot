using System;
using System.Collections.Generic;

namespace ScheduleBot.Domain.Interfaces
{
    public interface IWeekDatesProvider
    {
        IReadOnlyCollection<DateTime> GetWeekDates(DateTime dateTime, double days, DayOfWeek dayOfWeek);
    }
}
