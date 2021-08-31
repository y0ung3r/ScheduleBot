using System;

namespace ScheduleBot.Handlers.Interfaces
{
    public interface ICommandHandler : IRequestHandler
    {
        bool CanHandle(IServiceProvider serviceProvider, object request);
    }
}