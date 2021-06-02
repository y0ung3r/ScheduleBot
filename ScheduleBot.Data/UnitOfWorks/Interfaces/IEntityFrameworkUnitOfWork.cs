using System.Threading.Tasks;

namespace ScheduleBot.Data.UnitOfWorks.Interfaces
{
    public interface IEntityFrameworkUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
