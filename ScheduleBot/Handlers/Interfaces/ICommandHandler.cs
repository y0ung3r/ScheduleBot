namespace ScheduleBot.Handlers.Interfaces
{
    public interface ICommandHandler : IRequestHandler
    {
        bool CanHandle(object request);
    }
}