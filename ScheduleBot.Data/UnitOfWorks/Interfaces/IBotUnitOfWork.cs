using ScheduleBot.Data.Repositories.Interfaces;

namespace ScheduleBot.Data.UnitOfWorks.Interfaces
{
    public interface IBotUnitOfWork : IEntityFrameworkUnitOfWork
    {
        IChatParametersRepository ChatParameters { get; }
    }
}
