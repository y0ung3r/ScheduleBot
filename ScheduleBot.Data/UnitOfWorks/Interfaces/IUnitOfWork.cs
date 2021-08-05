using ScheduleBot.Data.Repositories.Interfaces;

namespace ScheduleBot.Data.UnitOfWorks.Interfaces
{
    public interface IUnitOfWork : IEntityFrameworkUnitOfWork
    {
        IChatParametersRepository ChatParameters { get; }
    }
}
