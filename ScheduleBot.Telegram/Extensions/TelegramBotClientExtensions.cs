using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Extensions
{
    public static class TelegramBotClientExtensions
    {
        public static async Task<Update> GetLastUpdateAsync(this ITelegramBotClient client, CancellationToken cancellationToken)
        {
            var update = default(Update);
            var emptyUpdates = Array.Empty<Update>();
            var messageOffset = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var timeout = (int)client.Timeout.TotalSeconds;
                var updates = emptyUpdates;

                try
                {
                    updates = await client.MakeRequestAsync
                                          (
                                              request: new GetUpdatesRequest()
                                              {
                                                  Offset = messageOffset,
                                                  Timeout = timeout
                                              },
                                              cancellationToken
                                          )
                                          .ConfigureAwait(false);
                }

                catch (OperationCanceledException)
                {
                    // Ignore
                }

                catch (Exception exception)
                {
                    throw new NotImplementedException();
                }

                update = updates.LastOrDefault();
            }

            return update;
        }
    }
}
