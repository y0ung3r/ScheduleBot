using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ScheduleBot.Telegram.LongPolling;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using Telegram.Bot;

namespace ScheduleBot.Telegram.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.TryAddSingleton<Application>();

            return services;
        }

        public static IServiceCollection AddTelegramBotClient(this IServiceCollection services, string token)
        {
            services.TryAddSingleton<ITelegramBotClient>
            (
                serviceProvider => new TelegramBotClient(token)
            );

            return services;
        }

        public static IServiceCollection AddTelegramLongPolling(this IServiceCollection services)
        {
            services.TryAddSingleton<IStepHandlerStorage, StepHandlerStorage>();
            services.TryAddSingleton<ILongPollingService, LongPollingService>();

            return services;
        }
    }
}
