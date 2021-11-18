using BotFramework.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Telegram.Interfaces;
using ScheduleBot.Telegram.LongPolling;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
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

            services.TryAddSingleton<ILongPoll, LongPoll>();

            services.TryAddScoped<ITelegramBot, ScheduleBot>();

            return services;
        }
    }
}
