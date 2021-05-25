using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;
using ScheduleBot.Telegram.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IReadOnlyCollection<ISystem<ITelegramBotClient>> _systems;
        private ISystem<ITelegramBotClient> _executedSystem;

        public TelegramBot(ITelegramBotClient client, IReadOnlyCollection<ISystem<ITelegramBotClient>> systems)
        {
            _client = client;
            _systems = systems;
        }

        private async Task OnUpdateReceivedAsync(Update update)
        {
            if (update.Type.Equals(UpdateType.Message))
            {
                var message = update.Message;
                var nextSystem = _systems.FirstOrDefault(system => system.MessageIsCommand(message.Text));

                if (nextSystem != null)
                {
                    _executedSystem = nextSystem;
                    
                    await _executedSystem.OnCommandReceivedAsync(message);
                }
                else
                {
                    await _executedSystem.OnMessageReceivedAsync(message);
                }
            }

            if (_executedSystem != null && _executedSystem is TelegramSystemBase system)
            {
                var updateHandler = update.Type switch
                {
                    UpdateType.EditedMessage => system.OnEditedMessageReceivedAsync(update.EditedMessage),
                    UpdateType.InlineQuery => system.OnInlineQueryReceivedAsync(update.InlineQuery),
                    UpdateType.CallbackQuery => system.OnCallbackQueryReceivedAsync(update.CallbackQuery),
                    _ => system.OnUnknownUpdateReceivedAsync(update)
                };

                await updateHandler;
            }
        }

        private Task OnErrorReceivedAsync(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API error [{apiRequestException.ErrorCode}]: {apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);

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

            Console.Title = _client.GetMeAsync()
                                   .GetAwaiter()
                                   .GetResult()
                                   .FirstName;

            Console.WriteLine("Bot is running");
            Console.ReadKey();

            _client.StopReceiving();
            cancellationTokenSource.Cancel();
        }
    }
}
