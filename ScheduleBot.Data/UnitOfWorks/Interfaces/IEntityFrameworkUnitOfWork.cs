using System.Threading.Tasks;

namespace ScheduleBot.Data.Interfaces.Repositories
{
    public interface IEntityFrameworkUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
