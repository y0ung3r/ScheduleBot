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
                            var header = $"<b>{lesson.Number}</b> {lesson.Title} ({lesson.Teacher})\n";
                            var content = $"<i>{lesson.Type} {lesson.ClassroomNumber}</i> в {lesson.TimeRange}";
                            return header + content;
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
                    "Вы не настроили бота, чтобы использовать этот функционал.\nИспользуйте /start, чтобы начать",
                    ParseMode.Html
                );
            }
        }
    }
}
