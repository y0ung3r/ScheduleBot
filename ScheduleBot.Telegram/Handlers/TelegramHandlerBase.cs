using BotFramework;
using BotFramework.Handlers.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public abstract class TelegramHandlerBase : IRequestHandler
    {
        protected abstract Task HandleAsync(Update update, RequestDelegate nextHandler);

        public Task HandleAsync(object request, RequestDelegate nextHandler)
        {
            if (request is Update update)
            {
                return HandleAsync(update, nextHandler);
            }

            return nextHandler(request);
        }
    }
}
