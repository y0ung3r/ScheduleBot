using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.Extensions;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public class TelegramExceptionHandler : TelegramHandlerBase
    {
        private readonly ILogger<TelegramExceptionHandler> _logger;
        private readonly ITelegramBotClient _client;

        public TelegramExceptionHandler(ILogger<TelegramExceptionHandler> logger, ITelegramBotClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task HandleAsync(Update update, RequestDelegate nextHandler)
        {
            try
            {
                await nextHandler(update);
            }

            catch (Exception exception)
            {
                var errorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error ({apiRequestException.ErrorCode}):\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                if (update.GetChatId() is long chatId)
                {
                    await _client.SendTextMessageAsync
                    (
                        chatId,
                        text: "Произошла непредвиденная ошибка. Невозможно обработать Ваш запрос"
                    );
                }

                _logger?.LogError(errorMessage);
            }
        }
    }
}
