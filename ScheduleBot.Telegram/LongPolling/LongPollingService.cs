using BotFramework;
using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using ScheduleBot.Telegram.StepHandler;
using ScheduleBot.Telegram.StepHandler.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.LongPolling
{
    public class LongPollingService : ILongPollingService
    {
        private readonly ILogger<LongPollingService> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IStepRequestStorage _stepRequestStorage;

        public LongPollingService(ILogger<LongPollingService> logger, ITelegramBotClient client, IStepRequestStorage stepRequestStorage)
        {
            _logger = logger;
            _client = client;
            _stepRequestStorage = stepRequestStorage;
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

                        if (update.Message is Message message)
                        {
                            _stepRequestStorage.RemoveChatRequests(message.Chat.Id);
                        }

                        if (update.GetChatId() is long chatId && update.GetRequestMessageId() is int requestMessageId && _stepRequestStorage.GetRequestInfo(chatId, requestMessageId) is StepRequestInfo requestInfo)
                        {
                            _stepRequestStorage.RemoveChatRequests(chatId);

                            await requestInfo.Callback
                            (
                                request: requestInfo.Message,
                                response: update,
                                payload: requestInfo.Payload
                            );
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
