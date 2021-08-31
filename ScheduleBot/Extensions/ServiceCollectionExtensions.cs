using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Handlers;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Interfaces;
using System;

namespace ScheduleBot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBot<TBot>(this IServiceCollection services)
            where TBot : IBot
        {
            services.TryAddTransient<IBranchBuilder, BranchBuilder>();

            services.TryAddTransient<Func<RequestDelegate, IBot>>
            (
                serviceProvider => branch => ActivatorUtilities.CreateInstance<TBot>(serviceProvider, branch)
            );

            services.TryAddTransient<Func<RequestDelegate, Predicate<object>, InternalHandler>>
            (
                serviceProvider => (branch, predicate) => ActivatorUtilities.CreateInstance<InternalHandler>(serviceProvider, branch, predicate)
            );

            return services;
        }

        public static IServiceCollection AddHandler<TRequestHandler>(this IServiceCollection services)
            where TRequestHandler : IRequestHandler
        {
            services.TryAddTransient
            (
                typeof(TRequestHandler),
                typeof(TRequestHandler)
            );

            return services;
        }
    }
}
