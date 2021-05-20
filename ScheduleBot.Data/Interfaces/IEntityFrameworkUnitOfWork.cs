using System.Threading.Tasks;

namespace ScheduleBot.Data.Interfaces
{
    public interface IEntityFrameworkUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
