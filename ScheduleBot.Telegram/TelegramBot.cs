﻿using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TelegramBot> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IReadOnlyCollection<ISystem<ITelegramBotClient>> _systems;
        private ISystem<ITelegramBotClient> _executedSystem;

        public TelegramBot(ILogger<TelegramBot> logger, ITelegramBotClient client, IReadOnlyCollection<ISystem<ITelegramBotClient>> systems)
        {
            _logger = logger;
            _client = client;
            _systems = systems;
        }

        private async Task OnUpdateReceivedAsync(Update update)
        {
            _logger.LogInformation("Update received");

            if (update.Type.Equals(UpdateType.Message))
            {
                var message = update.Message;
                var nextSystem = _systems.FirstOrDefault(system => system.MessageIsCommand(message.Text));

                if (nextSystem != null)
                {
                    _executedSystem = nextSystem;
                    
                    await _executedSystem.OnCommandReceivedAsync(message);

                    _logger.LogInformation($"Command sended to the system \"{_executedSystem.GetType()}\"");
                }
                else if (_executedSystem != null)
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

                _logger.LogInformation($"Updates sended to the system \"{system.GetType()}\"");

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

            _logger.LogError(errorMessage);

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

            _logger.LogInformation("Bot is running");

            Console.ReadKey();

            _client.StopReceiving();
            cancellationTokenSource.Cancel();
        }
    }
}
