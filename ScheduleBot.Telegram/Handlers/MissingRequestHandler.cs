using BotFramework;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public class MissingRequestHandler : RequestHandlerBase<Update>
    {
        public override Task HandleAsync(Update request, RequestDelegate nextHandler)
        {
            throw new System.NotImplementedException();
        }
    }
}
