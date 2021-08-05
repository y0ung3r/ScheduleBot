using ScheduleBot.Telegram.LongPolling;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram.Extensions
{
    public static class LongPollingServiceExtensions
    {
        public static void Receive(this ILongPollingService longPollingService, RequestDelegate rootHandler,
            CancellationToken cancellationToken = default)
        {
            if (rootHandler is null)
            {
                throw new ArgumentNullException(nameof(rootHandler));
            }

            Task.Run(async () =>
            {
                await longPollingService.ReceiveAsync(rootHandler, cancellationToken);
            });
        }

        public static void RegisterStepHandler(this ILongPollingService longPollingService, long chatId, StepDelegate callback, 
            object payload = null)
        {
            longPollingService.StepHandlerStorage.RegisterStepHandler(chatId, callback, payload);
        }
    }
}
