using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Parser.Interfaces;
using System.Net.Http;

namespace ScheduleBot.Parser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduleParser<TScheduleParser>(this IServiceCollection services)
            where TScheduleParser : IScheduleParser
        {
            services.TryAddSingleton
            (
                typeof(IScheduleParser),
                serviceProvider => ActivatorUtilities.CreateInstance<TScheduleParser>
                (
                    serviceProvider, 
                    new HttpClient()
                )
            );

            return services;
        }
    }
}
