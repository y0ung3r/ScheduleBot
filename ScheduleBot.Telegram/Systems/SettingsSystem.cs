using ScheduleBot.Attributes;
using ScheduleBot.Parser.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/settings")]
    public class SettingsSystem : SystemBase
    {
        private readonly IScheduleParser _scheduleParser;

        public SettingsSystem(IScheduleParser scheduleParser)
        {
            _scheduleParser = scheduleParser;
        }

        public override async Task OnCommandReceivedAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var userSchedule = await Bot.UnitOfWork.UserSchedules.FindUserSchedule(chatId);

            if (userSchedule != null)
            {
                var facultyId = userSchedule.FacultyId;
                var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);
                var group = await _scheduleParser.ParseGroupAsync(facultyId, userSchedule.GroupId, userSchedule.GroupTypeId);

                await Bot.Client.SendTextMessageAsync
                (
                    chatId,
                    "<b>Текущие настройки:</b>\n" +
                    $"Факультет: <i>{faculty.TitleWithoutFacultyTag}</i>\n" +
                    $"Группа: <i>{group.Title}</i>",
                    ParseMode.Html
                );
            }
            else
            {
                await Bot.Client.SendTextMessageAsync
                (
                    chatId,
                    "<b>Текущие настройки:</b>\n" +
                    "Отсутствуют\n" +
                    "Используйте /setup, чтобы начать работу с ботом",
                    ParseMode.Html
                );
            }
        }
    }
}
