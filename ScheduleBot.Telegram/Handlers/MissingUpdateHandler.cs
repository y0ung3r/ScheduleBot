using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Extensions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers
{
    public class MissingUpdateHandler : TelegramHandlerBase
    {
        private readonly ILogger<MissingUpdateHandler> _logger;
        private readonly ITelegramBotClient _client;

        public MissingUpdateHandler(ILogger<MissingUpdateHandler> logger, ITelegramBotClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task HandleAsync(Update update, RequestDelegate nextHandler)
        {
            _logger.LogWarning($"No handler for request with type: {update.Type}");
            
            if (update.CallbackQuery is CallbackQuery callbackQuery)
            {
                await _client.AnswerCallbackQueryAsync
                (
                    callbackQuery.Id,
                    text: "Невозможно обработать Ваш запрос"
                );
            }
            else if (update.GetChatId() is long chatId && update.GetChatType() is not ChatType.Group)
            {
                await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Некорректный запрос. Используйте /help для получения списка доступных команд"
                );
            }
        }
    }
}
