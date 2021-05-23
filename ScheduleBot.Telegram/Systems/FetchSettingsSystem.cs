using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Parser.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/settings")]
    public class FetchSettingsSystem : SystemBase
    {
        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;

        public FetchSettingsSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
        }

        public override async Task OnCommandReceivedAsync(ITelegramBotClient client, Message command)
        {
            var chatId = command.Chat.Id;
            var userSchedule = await _unitOfWork.UserSchedules.FindUserSchedule(chatId);

            if (userSchedule != null)
            {
                var facultyId = userSchedule.FacultyId;
                var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);
                var group = await _scheduleParser.ParseGroupAsync(facultyId, userSchedule.GroupId, userSchedule.GroupTypeId);

                await client.SendTextMessageAsync
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
                await client.SendTextMessageAsync
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
