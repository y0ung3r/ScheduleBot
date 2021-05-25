using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telegram.Bot;

namespace ScheduleBot.Telegram.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelegramClient(this IServiceCollection services, string token)
        {
            services.AddScoped<ITelegramBotClient, TelegramBotClient>
            (
                serviceProvider => new TelegramBotClient(token)
            );

            return services;
        }

        public static IServiceCollection AddTelegramBot(this IServiceCollection services, ICollection<Type> systemTypes)
        {
            services.AddScoped<IBot, TelegramBot>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<TelegramBot>>();
                var client = serviceProvider.GetRequiredService<ITelegramBotClient>();

                var readOnlySystems = new ReadOnlyCollection<ISystem<ITelegramBotClient>>
                (
                    systemTypes.Select(systemType =>
                    {
                        var system = serviceProvider.GetRequiredService(systemType) as ISystem<ITelegramBotClient>;

                        systemType.BaseType
                                  .GetProperty(nameof(system.Client))
                                  .SetValue(system, client);

                        system.OnInitializeAsync()
                              .GetAwaiter()
                              .GetResult();

                        return system;
                    })
                    .ToList()
                ); 

                return new TelegramBot
                (
                    logger,
                    client,
                    readOnlySystems
                );
            });

            return services;
        }
    }
}
