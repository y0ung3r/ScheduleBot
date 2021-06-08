using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Interfaces;
using ScheduleBot.States.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram
{
    public class TelegramBot : IBot
    {
        private readonly ITelegramBotClient _client;
        private readonly ICommandManager _commandManager;
        private readonly IStateProvider _stateProvider;
        private readonly DefaultUpdateHandler _updateHandler;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TelegramBot(ITelegramBotClient client, ICommandManager commandManager, IStateProvider stateProvider)
        {
            _client = client;
            _commandManager = commandManager;
            _stateProvider = stateProvider;

            _updateHandler = new DefaultUpdateHandler
            (
                (_, update, _) => OnUpdateReceivedAsync(update),
                (_, exception, _) => OnErrorReceivedAsync(exception)
            );

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task OnUpdateReceivedAsync(Update update)
        {
            if (update.Type is UpdateType.Message)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;
                var route = message.Text;

                if (_commandManager.TryFindCommand(route, out IBotCommand command))
                {
                    await command.ExecuteAsync();
                }
            }

            await Task.CompletedTask;
        }

        private Task OnErrorReceivedAsync(Exception exception)
        {
            var errorText = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API error [{apiRequestException.ErrorCode}]: {apiRequestException.Message}",
                _ => exception.ToString()
            };

            return Task.CompletedTask;
        }

        public void Start()
        {
            _client.StartReceiving
            (
                _updateHandler,
                _cancellationTokenSource.Token
            );
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
