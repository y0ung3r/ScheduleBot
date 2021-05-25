using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Parser.Interfaces;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Systems
{
    [Command(pattern: "/settings")]
    public class FetchSettingsSystem : TelegramSystemBase
    {
        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;

        public FetchSettingsSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
        }

        protected override async Task OnCommandReceivedAsync(Message command)
        {
            var chatId = command.Chat.Id;
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParameters(chatId);

            var stringBuilder = new StringBuilder();

            if (chatParameters != null)
            {
                var facultyId = chatParameters.FacultyId;
                var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);
                var group = await _scheduleParser.ParseGroupAsync(facultyId, chatParameters.GroupId, chatParameters.GroupTypeId);

                stringBuilder.AppendLine("<b>Текущие настройки:</b>")
                             .AppendLine($"Факультет: <i>{faculty.TitleWithoutFacultyTag}</i>")
                             .AppendLine($"Группа: <i>{group.Title}</i>");
            }
            else
            {
                stringBuilder.AppendLine("<b>Текущие настройки:</b>")
                             .AppendLine($"Отсутствуют")
                             .AppendLine($"Используйте /setup, чтобы начать работу с ботом");
            }

            await Client.SendTextMessageAsync
            (
                chatId,
                stringBuilder.ToString(),
                ParseMode.Html
            );
        }
    }
}
