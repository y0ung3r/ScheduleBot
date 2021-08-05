using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands
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

            stringBuilder.AppendLine("Вы можете использовать команды из следующих категорий:")
                         .AppendLine()
                         .AppendLine("<b>Настройки:</b>")
                         .AppendLine("/settings - получить текущие настройки факультета и группы")
                         .AppendLine("/bind - изменить настройки факультета и группы")
                         .AppendLine()
                         .AppendLine("<b>Возможности:</b>")
                         .AppendLine("/schedule - получить актуальное расписание по заданной учебной неделе")
                         .AppendLine("/tomorrow - получить расписание на завтра");

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
