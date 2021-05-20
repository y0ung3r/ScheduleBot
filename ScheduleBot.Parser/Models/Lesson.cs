using System.Collections.Generic;

namespace ScheduleBot.Parser.Models
{
    public class Lesson 
    {
        public string Number { get; set; }

        public string Title { get; set; }

        public string TimeRange { get; set; }

        public string Type { get; set; }

        public string ClassroomNumber { get; set; }

        public ICollection<string> Teachers { get; set; }
    }
}
