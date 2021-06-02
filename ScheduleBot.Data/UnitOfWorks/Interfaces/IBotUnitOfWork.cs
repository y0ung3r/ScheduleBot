namespace ScheduleBot.Data.Interfaces.Repositories
{
    public interface IBotUnitOfWork : IEntityFrameworkUnitOfWork
    {
        IChatParametersRepository ChatParameters { get; }
    }
}
