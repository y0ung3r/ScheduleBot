namespace ScheduleBot.Commands.Interfaces
{
    public interface ICommandManager
    {
        void RegisterCommand(IBotCommand command);

        void UnregisterCommand(IBotCommand command);

        IBotCommand FindCommand(string route);

        bool TryFindCommand(string route, out IBotCommand command);
    }
}
