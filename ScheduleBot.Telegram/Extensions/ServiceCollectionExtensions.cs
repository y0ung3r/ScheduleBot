using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBot(this IServiceCollection services, string token, ICollection<Type> systemTypes)
        {
            services.AddScoped<IBot, Bot>(serviceProvider =>
            {
                return new Bot
                (
                    token,
                    new ReadOnlyCollection<ISystem>
                    (
                        systemTypes.Select(systemType => serviceProvider.GetRequiredService(systemType) as ISystem)
                                   .ToList()
                    ),
                    serviceProvider.GetRequiredService<IBotUnitOfWork>()
                );
            });

            return services;
        }
    }
}
