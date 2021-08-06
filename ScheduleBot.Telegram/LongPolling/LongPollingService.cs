using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using ScheduleBot.Telegram.ReplyService;
using ScheduleBot.Telegram.ReplyService.Interfaces;
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
        private readonly IReplyMessageService _replyMessageService;

        public LongPollingService(ILogger<LongPollingService> logger, ITelegramBotClient client, IReplyMessageService replyMessageService)
        {
            _logger = logger;
            _client = client;
            _replyMessageService = replyMessageService;
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

                        if (update.GetChatId() is long chatId && _replyMessageService.GetRequestInfo(chatId) is RequestInfo requestInfo)
                        {
                            await requestInfo.Callback(requestInfo.Message, update, requestInfo.Payload);

                            _replyMessageService.UnregisterRequest(chatId);
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
