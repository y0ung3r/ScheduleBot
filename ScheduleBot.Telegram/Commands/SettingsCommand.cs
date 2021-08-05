using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands
{
    [CommandText("/settings")]
    public class SettingsCommand : TelegramCommandBase
    {
        private readonly ILogger<SettingsCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IChatParametersService _chatParametersService;

        public SettingsCommand(ILogger<SettingsCommand> logger, ITelegramBotClient client, IChatParametersService chatParametersService)
        {
            _logger = logger;
            _client = client;
            _chatParametersService = chatParametersService;
        }

        protected override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var chatId = message.Chat.Id;
            var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<b>Текущие настройки:</b>");

            if (chatParameters is not null)
            {
                stringBuilder.AppendLine($"Факультет: <i>{chatParameters.FacultyTitleWithoutTag}</i>")
                             .AppendLine($"Группа: <i>{chatParameters.GroupTitle}</i>");
            }
            else
            {
                stringBuilder.AppendLine($"Отсутствуют")
                             .AppendLine($"Используйте /bind, чтобы начать работу с ботом");
            }

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            await _client.SendTextMessageAsync
            (
                chatId,
                text: stringBuilder.ToString(),
                parseMode: ParseMode.Html
            );

            _logger?.LogInformation("Settings command processed");
        }
    }
}
