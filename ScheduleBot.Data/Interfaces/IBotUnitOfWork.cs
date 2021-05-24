namespace ScheduleBot.Data.Interfaces
{
    public interface IBotUnitOfWork : IEntityFrameworkUnitOfWork
    {
        IChatParametersRepository ChatParameters { get; }
    }
}
