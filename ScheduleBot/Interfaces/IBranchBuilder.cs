using ScheduleBot.Handlers.Interfaces;
using System;

namespace ScheduleBot.Interfaces
{
    public interface IBranchBuilder
    {
        IServiceProvider ServiceProvider { get; }

        IBranchBuilder UseHandler(IRequestHandler handler);

        IBranchBuilder UseBranch(Predicate<object> predicate, Action<IBranchBuilder> configure);

        RequestDelegate Build();
    }
}
