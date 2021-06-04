using ScheduleBot.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram
{
    public class TelegramBot : IBot, IRunnable
    {
        private readonly ITelegramBotClient _client;

        public TelegramBot(ITelegramBotClient client)
        {
            _client = client;
        }

        private async Task ProcessUpdateAsync(Update update)
        {


            await Task.CompletedTask;
        }

        public async Task OnUpdateReceivedAsync(object update)
        {
            await ProcessUpdateAsync(update as Update);
        }

        public Task OnErrorReceivedAsync(Exception exception)
        {
            var errorText = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API error [{apiRequestException.ErrorCode}]: {apiRequestException.Message}",
                _ => exception.ToString()
            };

            return Task.CompletedTask;
        }

        public void Run()
        {
            var updateHandler = new DefaultUpdateHandler
            (
                (_, update, _) => OnUpdateReceivedAsync(update),
                (_, exception, _) => OnErrorReceivedAsync(exception)
            );

            var cancellationTokenSource = new CancellationTokenSource();

            _client.StartReceiving
            (
                updateHandler,
                cancellationTokenSource.Token
            );

            Console.WriteLine("Bot is running...");
            Console.ReadKey();

            cancellationTokenSource.Cancel();
        }
    }
}
