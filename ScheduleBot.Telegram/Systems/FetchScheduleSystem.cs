using ScheduleBot.Attributes;
using ScheduleBot.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/schedule")]
    public class FetchScheduleSystem : SystemBase
    {
        private readonly IScheduleParser _scheduleParser;

        public FetchScheduleSystem(IScheduleParser scheduleParser)
        {
            _scheduleParser = scheduleParser;
        }

        public override async Task OnCommandReceivedAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var userSchedule = await Bot.UnitOfWork.UserSchedules.FindUserSchedule(chatId);

            if (userSchedule != null)
            {
                var userGroup = await _scheduleParser.ParseGroupAsync(userSchedule.FacultyId, userSchedule.GroupId, userSchedule.GroupTypeId);

                var dateTime = DateTime.Now;
                var lessons = await _scheduleParser.ParseLessonsAsync(userGroup, dateTime);

                var lessonsMarkup = new List<string>()
                {
                    $"<b>Расписание на {dateTime.ToShortDateString()}:</b>"
                };

                if (lessons.Count > 0)
                {
                    lessonsMarkup.AddRange
                    (
                        lessons.Select(lesson =>
                        {
                            var header = $"<b>{lesson.Number} {lesson.Title}</b>\n";
                            var type = string.Empty;

                            if (!string.IsNullOrWhiteSpace(lesson.Type))
                            {
                                type = $"<i>{lesson.Type}</i>";
                            }

                            var classroomNumber = string.Empty;

                            if (!string.IsNullOrWhiteSpace(lesson.ClassroomNumber))
                            {
                                classroomNumber = $" <i>{lesson.ClassroomNumber}</i> в ";
                            }

                            var additionalInfo = string.Concat(type, classroomNumber, $"{lesson.TimeRange}\n");

                            var teachers = string.Empty;

                            if (lesson.Teachers.Count > 0)
                            {
                                teachers = $"{string.Join(separator: ", ", lesson.Teachers)}";
                            }

                            return string.Concat(header, additionalInfo, teachers);
                        })
                    );
                }
                else
                {
                    lessonsMarkup.Add("Пары отсутствуют");
                }

                await Bot.Client.SendTextMessageAsync
                (
                    chatId,
                    string.Join("\n\n", lessonsMarkup),
                    ParseMode.Html
                );
            }
            else
            {
                await Bot.Client.SendTextMessageAsync
                (
                    chatId,
                    "Вы не настроили бота, чтобы использовать этот функционал.\nИспользуйте /setup, чтобы начать работу",
                    ParseMode.Html
                );
            }
        }
    }
}
