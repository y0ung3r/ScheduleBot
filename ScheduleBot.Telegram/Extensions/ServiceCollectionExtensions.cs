using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Telegram.LongPolling;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using ScheduleBot.Telegram.StepHandler;
using ScheduleBot.Telegram.StepHandler.Interfaces;
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

        public static IServiceCollection AddTelegramBotExtensions(this IServiceCollection services)
        {
            services.TryAddSingleton<IStepRequestStorage, StepRequestStorage>();
            services.TryAddSingleton<ILongPollingService, LongPollingService>();

            return services;
        }
    }
}
