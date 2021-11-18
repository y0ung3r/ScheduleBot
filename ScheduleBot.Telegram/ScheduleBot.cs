using BotFramework;
using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Interfaces;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram
{
    public class ScheduleBot : ITelegramBot
    {
        private readonly ILogger<ScheduleBot> _logger;
        private readonly RequestDelegate _branch;
        private readonly ITelegramBotClient _client;
        private readonly ILongPoll _longPoll;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ScheduleBot(ILogger<ScheduleBot> logger, RequestDelegate branch, ITelegramBotClient client, ILongPoll longPoll)
        {
            _logger = logger;
            _branch = branch;
            _client = client;
            _longPoll = longPoll;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task<User> GetBotInfoAsync()
        {
            return _client.GetMeAsync();
        }

        public Task RunAsync()
        {
            _logger?.LogInformation("Bot is running...");

            return _longPoll.ReceiveAsync
            (
                _branch,
                _cancellationTokenSource.Token
            );
        }

        public void Stop()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _logger?.LogInformation("Bot stopped");

                _cancellationTokenSource.Cancel();
            }
        }
    }
}
