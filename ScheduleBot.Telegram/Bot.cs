using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot
{
    public class Bot : IBot
    {
        private readonly string _token;
        private readonly IReadOnlyCollection<ISystem> _systems;

        public Bot(string token, IReadOnlyCollection<ISystem> systems)
        {
            _token = token;
            _systems = systems;
        }

        private async Task OnUpdateReceivedAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            foreach (var system in _systems)
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var message = update.Message;

                        if (system.MessageIsCommand(message.Text))
                        {
                            await system.OnCommandReceivedAsync(client, message);
                        }
                        else
                        {
                            await system.OnMessageReceivedAsync(client, message);
                        }
                        break;

                    case UpdateType.EditedMessage:
                        await system.OnEditedMessageReceivedAsync(client, update.EditedMessage);
                        break;

                    case UpdateType.InlineQuery:
                        await system.OnInlineQueryReceivedAsync(client, update.InlineQuery);
                        break;

                    case UpdateType.CallbackQuery:
                        await system.OnCallbackQueryReceivedAsync(client, update.CallbackQuery);
                        break;

                    default:
                        await system.OnUnknownUpdateReceivedAsync(client, update);
                        break;
                }
            }
        }

        private Task OnErrorReceivedAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
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
            var client = new TelegramBotClient(_token);
            var updateHandler = new DefaultUpdateHandler(OnUpdateReceivedAsync, OnErrorReceivedAsync);
            var cancellationTokenSource = new CancellationTokenSource();

            client.StartReceiving
            (
                updateHandler,
                cancellationTokenSource.Token
            );

            Console.WriteLine("Bot is running");
            Console.ReadKey();

            client.StopReceiving();

            cancellationTokenSource.Cancel();
        }
    }
}
