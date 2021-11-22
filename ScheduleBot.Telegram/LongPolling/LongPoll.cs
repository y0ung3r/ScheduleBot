using BotFramework;
using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using ScheduleBot.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.LongPolling
{
    public class LongPoll : ILongPoll
    {
        private readonly ILogger<LongPoll> _logger;
        private readonly ITelegramBotClient _client;

        public LongPoll(ILogger<LongPoll> logger, ITelegramBotClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task ReceiveAsync(RequestDelegate rootHandler, CancellationToken cancellationToken = default)
        {
            if (rootHandler is null)
            {
                throw new ArgumentNullException(nameof(rootHandler));
            }

            var messageOffset = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var updates = Array.Empty<Update>();

                try
                {
                    updates = await _client.GetUpdatesAsync
                    (
                        offset: messageOffset,
                        timeout: (int)_client.Timeout.TotalSeconds,
                        cancellationToken: cancellationToken
                    );
                }

                catch (OperationCanceledException)
                {
                    // Ignore...
                }

                finally
                {
                    foreach (var update in updates)
                    {
                        _logger?.LogInformation($"Update received with type: {update.Type}");

                        await rootHandler(update);

                        messageOffset = update.Id + 1;
                    }
                }
            }
        }
    }
}
