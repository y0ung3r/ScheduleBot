using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands
{
    [CommandText("/help, /start")]
    public class StartCommand : TelegramCommandBase
    {
        private readonly ILogger<StartCommand> _logger;
        private readonly ITelegramBotClient _client;

        public StartCommand(ILogger<StartCommand> logger, ITelegramBotClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("<b>С помощью данного бота Вы можете:</b>")
                         .AppendLine("• обеспечить себе быстрый доступ к расписанию, используя короткую команду /bind;")
                         .AppendLine("• добавить бота в беседу группы, чтобы делиться расписанием с одногруппниками;")
                         .AppendLine("• использовать нижеуказанные команды.")
                         .AppendLine()
                         .AppendLine("<b>/settings</b> - получить текущие настройки факультета и группы")
                         .AppendLine()
                         .AppendLine("<b>/bind</b> - изменить настройки факультета и группы")
                         .AppendLine()
                         .AppendLine("<b>/schedule</b> - получить актуальное расписание по заданной учебной неделе")
                         .AppendLine()
                         .AppendLine("<b>/schedule <i>[дата (в любом формате)]</i></b> - получить актуальное расписание по заданному учебному дню")
                         .AppendLine("<i>Пример использования:</i> <code>/schedule 01.09.2021</code>")
                         .AppendLine()
                         .AppendLine("<b>/schedule <i>[дата (в любом формате)]</i> <i>[название группы]</i></b> - получить актуальное расписание по заданному учебному дню")
                         .AppendLine("<i>Пример использования:</i> <code>/schedule 2021-9-1 ПМИ31</code>")
                         .AppendLine()
                         .AppendLine("<b>/tomorrow</b> - получить расписание на завтра")
                         .AppendLine()
                         .AppendLine("<b>/tomorrow <i>[название группы]</i></b> - получить расписание на завтра для указанной группы.")
                         .AppendLine("<i>Пример использования:</i> <code>/tomorrow ХИМ21</code>");

            var chatId = message.Chat.Id;

            await _client.SendChatActionAsync
            (
                chatId, 
                chatAction: ChatAction.Typing
            );

            await _client.SendTextMessageAsync
            (
                chatId,
                text: stringBuilder.ToString(),
                parseMode: ParseMode.Html,
                disableWebPagePreview: true
            );

            _logger?.LogInformation("Help/Start command processed");
        }
    }
}
