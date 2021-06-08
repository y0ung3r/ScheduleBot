namespace ScheduleBot.States.Interfaces
{
    public interface IStateProvider
    {
        IBotState CreateState<TState>()
            where TState : IBotState;
    }
}
