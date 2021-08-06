using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public class UnknownMessageHandler : TelegramHandlerBase
    {
        private readonly ITelegramBotClient _client;

        public UnknownMessageHandler(ITelegramBotClient client)
        {
            _client = client;
        }

        protected override async Task HandleAsync(Update update, RequestDelegate nextHandler)
        {
            var message = update.Message;

            if (message is not null)
            {
                await _client.SendTextMessageAsync
                (
                    message.Chat.Id,
                    text: $"Невозможно использовать \"{message.Text}\" в качестве команды"
                );
            }

            await nextHandler(update);
        }
    }
}
