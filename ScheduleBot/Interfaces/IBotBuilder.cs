namespace ScheduleBot.Interfaces
{
    public interface IBotBuilder
    {
        IBotBuilder Use<TBot>() 
            where TBot : IBot;

        IBotBuilder WithStartup<TStartup>() 
            where TStartup : IStartup;

        IBotBuilder WithCommand<TCommand>()
            where TCommand : class;

        IRunnable Build();
    }
}
