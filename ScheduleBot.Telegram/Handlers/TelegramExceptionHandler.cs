using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public class TelegramExceptionHandler : TelegramHandlerBase
    {
        private readonly ILogger<TelegramExceptionHandler> _logger;

        public TelegramExceptionHandler(ILogger<TelegramExceptionHandler> logger)
        {
            _logger = logger;
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

                _logger?.LogError(errorMessage);
            }
        }
    }
}
