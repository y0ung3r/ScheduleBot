using ScheduleBot.Handlers.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram.LongPolling.Interfaces
{
    public interface ILongPollingService
    {
        IStepHandlerStorage StepHandlerStorage { get; }

        Task ReceiveAsync(RequestDelegate rootHandler, CancellationToken cancellationToken = default);
    }
}
