using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Telegram.Interfaces;
using ScheduleBot.Telegram.LongPolling;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using BotFramework;
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
                _ => new TelegramBotClient(token)
            );

            services.TryAddSingleton<ILongPoll, LongPoll>();

            services.TryAddScoped<Func<RequestDelegate, ITelegramBot>>
            (
                serviceProvider => branch => ActivatorUtilities.CreateInstance<ScheduleBot>(serviceProvider, branch)
            );

            return services;
        }
    }
}
