using BotFramework;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram.LongPolling.Extensions
{
    public static class LongPollExtensions
    {
        public static void Receive(this ILongPoll longPoll, RequestDelegate rootHandler,
            CancellationToken cancellationToken = default)
        {
            if (rootHandler is null)
            {
                throw new ArgumentNullException(nameof(rootHandler));
            }

            Task.Run
            (
                async () =>
                {
                    await longPoll.ReceiveAsync(rootHandler, cancellationToken);
                }, 
                cancellationToken
            );
        }
    }
}
