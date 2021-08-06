using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public class UnknownCallbackQueryHandler : TelegramHandlerBase
    {
        private readonly ITelegramBotClient _client;

        public UnknownCallbackQueryHandler(ITelegramBotClient client)
        {
            _client = client;
        }

        protected override async Task HandleAsync(Update update, RequestDelegate nextHandler)
        {
            var callbackQuery = update.CallbackQuery;
            var answerMessage = "Невозможно обработать Ваш запрос";

            if (callbackQuery is not null)
            {
                await _client.AnswerCallbackQueryAsync
                (
                    callbackQuery.Id,
                    text: answerMessage
                );
            }

            await nextHandler(update);
        }
    }
}
