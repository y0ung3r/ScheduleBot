using Microsoft.Extensions.Logging;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.LongPolling
{
    public class LongPollingService : ILongPollingService
    {
        private readonly ILogger<LongPollingService> _logger;
        private readonly ITelegramBotClient _client;

        public IStepHandlerStorage StepHandlerStorage { get; }

        public LongPollingService(ILogger<LongPollingService> logger, ITelegramBotClient client, IStepHandlerStorage stepHandlerStorage)
        {
            _logger = logger;
            _client = client;

            StepHandlerStorage = stepHandlerStorage;
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

                        if (update.GetChatId() is long chatId)
                        {
                            var isCommand = update.IsCommand();

                            if (isCommand)
                            {
                                StepHandlerStorage.ClearChatStepHandler(chatId);
                            }

                            if (StepHandlerStorage.IsStepHandlerRegistered(chatId))
                            {
                                var stepHandlerInfo = StepHandlerStorage.GetStepHandlerInfo(chatId);
                                StepHandlerStorage.ClearChatStepHandler(chatId);

                                await stepHandlerInfo.Callback.Invoke(update, stepHandlerInfo.Payload);
                            }
                            else
                            {
                                await rootHandler(update);
                            }
                        }
                        else
                        {
                            await rootHandler(update);
                        }

                        messageOffset = update.Id + 1;
                    }
                }
            }
        }
    }
}
