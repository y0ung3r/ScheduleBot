using BotFramework;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram.LongPolling.Interfaces
{
    public interface ILongPollingService
    {
        Task ReceiveAsync(RequestDelegate rootHandler, CancellationToken cancellationToken = default);
    }
}
