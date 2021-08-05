using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Domain.Interfaces;

namespace ScheduleBot.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.TryAddTransient<IChatParametersService, ChatParametersService>();

            return services;
        }
    }
}
