using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Interfaces;
using System;

namespace ScheduleBot.Extensions
{
    public static class BranchBuilderExtensions
    {
        public static IBranchBuilder UseInternalHandler<TRequestHandler>(this IBranchBuilder builder, Predicate<object> predicate)
            where TRequestHandler : IRequestHandler
        {
            return builder.UseInternalHandler
            (
                predicate,
                branchBuilder => branchBuilder.UseHandler<TRequestHandler>()
            );
        }

        public static IBranchBuilder UseCommand<TCommandHandler>(this IBranchBuilder builder)
            where TCommandHandler : ICommandHandler
        {
            return builder.UseInternalHandler
            (
                request => builder.ServiceProvider
                                  .GetRequiredService<TCommandHandler>()
                                  .CanHandle(request),

                branchBuilder => branchBuilder.UseHandler<TCommandHandler>()
            );
        }
    }
}
