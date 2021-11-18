using BotFramework;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers.Commands
{
    public abstract class TelegramCommandBase : CommandHandlerBase<Update>
    {
        public override Task HandleAsync(Update request, RequestDelegate nextHandler)
        {
            throw new NotImplementedException();
        }

        public override bool CanHandle(Update request)
        {
            throw new NotImplementedException();
        }
    }
}
