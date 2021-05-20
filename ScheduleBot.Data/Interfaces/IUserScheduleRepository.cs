using ScheduleBot.Data.Models;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Interfaces
{
    public interface IUserScheduleRepository : IEntityFrameworkRepository<UserSchedule>
    {
        Task<UserSchedule> FindUserSchedule(long chatId);
    }
}
