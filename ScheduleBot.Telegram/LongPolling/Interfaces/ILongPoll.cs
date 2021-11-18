using BotFramework;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram.LongPolling.Interfaces
{
    public interface ILongPoll
    {
        Task ReceiveAsync(RequestDelegate rootHandler, CancellationToken cancellationToken = default);
    }
}
