using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Interfaces;
using System;

namespace ScheduleBot.Extensions
{
    public static class BranchBuilderExtensions
    {
        public static IBranchBuilder UseHandler<TRequestHandler>(this IBranchBuilder builder)
            where TRequestHandler : IRequestHandler
        {
            return builder.UseHandler
            (
                builder.ServiceProvider.GetRequiredService<TRequestHandler>()
            );
        }

        public static IBranchBuilder UseAnotherBranch<TRequestHandler>(this IBranchBuilder builder, Predicate<object> predicate)
            where TRequestHandler : IRequestHandler
        {
            return builder.UseAnotherBranch
            (
                predicate,
                branchBuilder => branchBuilder.UseHandler<TRequestHandler>()
            );
        }

        public static IBranchBuilder UseCommand<TCommandHandler>(this IBranchBuilder builder)
            where TCommandHandler : ICommandHandler
        {
            var serviceProvider = builder.ServiceProvider;

            return builder.UseAnotherBranch<TCommandHandler>
            (
                request => serviceProvider.GetRequiredService<TCommandHandler>()
                                          .CanHandle(serviceProvider, request)
            );
        }
    }
}
