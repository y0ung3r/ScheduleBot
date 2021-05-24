using System.Collections.Generic;

namespace ScheduleBot.Parser.Models
{
    public class StudyDay
    {
        public int WeekNumber { get; set; }

        public ICollection<Lesson> Lessons { get; set; }

        public StudyDay()
        {
            Lessons = new List<Lesson>();
        }
    }
}
