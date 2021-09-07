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

            stringBuilder.AppendLine("Чтобы обеспечить себе быстрый доступ к расписанию, используйте команду /bind.")
                         .AppendLine("Также Вы можете добавить бота в свою Telegram-группу.")
                         .AppendLine()
                         .AppendLine("<i>Бот не хранит Ваши личные данные!</i>")
                         .AppendLine()
                         .AppendLine("<b>Вы можете использовать следующие команды:</b>")
                         .AppendLine()
                         .AppendLine("1. /settings - получить текущие настройки факультета и группы")
                         .AppendLine()
                         .AppendLine("2. /bind - изменить настройки факультета и группы")
                         .AppendLine()
                         .AppendLine("3. /schedule - получить актуальное расписание по заданной учебной неделе")
                         .AppendLine()
                         .AppendLine("4. /schedule <i>[дата (в любом формате)]</i> - получить актуальное расписание по заданному учебному дню")
                         .AppendLine("Пример: <code>/schedule 01.09.2021</code>")
                         .AppendLine()
                         .AppendLine("5. /schedule <i>[дата (в любом формате)]</i> <i>[название группы]</i> - получить актуальное расписание по заданному учебному дню")
                         .AppendLine("Пример: <code>/schedule 2021-9-1 ПМИ31</code>")
                         .AppendLine()
                         .AppendLine("6. /tomorrow - получить расписание на завтра")
                         .AppendLine()
                         .AppendLine("7. /tomorrow <i>[название группы]</i> - получить расписание на завтра для указанной группы.")
                         .AppendLine("Пример: <code>/tomorrow ХИМ21</code>");

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
