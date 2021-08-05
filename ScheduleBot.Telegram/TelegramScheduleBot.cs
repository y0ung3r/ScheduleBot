using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram
{
    public class TelegramScheduleBot : BotBase
    {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _client;
        private readonly ILongPollingService _longPollingService;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TelegramScheduleBot(RequestDelegate rootHandler, ILogger<TelegramScheduleBot> logger,
            ITelegramBotClient client, ILongPollingService longPollingService) : base(rootHandler)
        {
            _logger = logger;
            _client = client;
            _longPollingService = longPollingService;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task<User> GetBotInfoAsync()
        {
            return _client.GetMeAsync();
        }

        public override void Run()
        {
            _longPollingService.Receive
            (
                _rootHandler,
                _cancellationTokenSource.Token
            );

            _logger?.LogInformation("Bot is running...");
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();

            _logger?.LogInformation("Bot stopped");
        }
    }
}
