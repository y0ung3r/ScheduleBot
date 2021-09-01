using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleBot.Parser.Models
{
    public class StudyDay
    {
        public int WeekNumber { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Lesson> Lessons { get; set; }

        public StudyDay()
        {
            Lessons = new List<Lesson>();
        }

        public string ToHTML()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"<b>Расписание на {Date:dd.MM.yyyy (dddd)}:</b>")
                             .AppendLine();

            if (Lessons.Count > 0)
            {
                foreach (var lesson in Lessons)
                {
                    stringBuilder.AppendLine($"<b>{lesson.Number} {lesson.Title}</b>");

                    if (!string.IsNullOrWhiteSpace(lesson.Type))
                    {
                        stringBuilder.Append($"<i>{lesson.Type}</i>");

                        if (!string.IsNullOrWhiteSpace(lesson.ClassroomNumber))
                        {
                            stringBuilder.Append($" <i>{lesson.ClassroomNumber}</i>");
                        }

                        stringBuilder.Append($" в {lesson.TimeRange}");
                    }

                    stringBuilder.AppendLine();

                    if (lesson.Teachers.Count > 0)
                    {
                        stringBuilder.AppendJoin(", ", lesson.Teachers);
                    }

                    stringBuilder.AppendLine()
                                 .AppendLine();
                }
            }
            else
            {
                stringBuilder.AppendLine("Пары отсутствуют");
            }

            return stringBuilder.ToString();
        }
    }
}
