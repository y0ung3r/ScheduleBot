using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Parser.Interfaces;

namespace ScheduleBot.Parser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduleParser(this IServiceCollection services)
        {
            services.TryAddSingleton<IScheduleParser, ScheduleParser>();

            return services;
        }
    }
}
