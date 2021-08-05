using ScheduleBot.Parser.Models;
using System.Text;

namespace ScheduleBot.Telegram.Extensions
{
    public static class StudyDayExtensions
    {
        public static string ToHTML(this StudyDay studyDay)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"<b>Расписание на {studyDay.Date.ToShortDateString()}:</b>")
                             .AppendLine();

            if (studyDay.Lessons.Count > 0)
            {
                foreach (var lesson in studyDay.Lessons)
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
