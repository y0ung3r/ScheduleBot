namespace ScheduleBot.Data.Interfaces
{
    public interface IBotUnitOfWork : IEntityFrameworkUnitOfWork
    {
        IUserScheduleRepository UserSchedules { get; }
    }
}
