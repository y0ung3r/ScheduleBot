using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Data.Repositories;
using ScheduleBot.Data.Repositories.Interfaces;
using ScheduleBot.Data.UnitOfWorks.Interfaces;

namespace ScheduleBot.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TUnitOfWork>(this IServiceCollection services)
            where TUnitOfWork : IUnitOfWork
        {
            services.TryAddTransient
            (
                typeof(IUnitOfWork), 
                typeof(TUnitOfWork)
            );

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.TryAddTransient<IChatParametersRepository, ChatParametersRepository>();

            return services;
        }
    }
}
