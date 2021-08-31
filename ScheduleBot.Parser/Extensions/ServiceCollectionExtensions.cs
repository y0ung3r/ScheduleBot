using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Parser.Interfaces;

namespace ScheduleBot.Parser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduleParser<TScheduleParser>(this IServiceCollection services)
            where TScheduleParser : IScheduleParser
        {
            services.TryAddSingleton<IRestClient, RestClient>();
            services.TryAddSingleton<IScheduleParser, ScheduleParser>();

            return services;
        }
    }
}
