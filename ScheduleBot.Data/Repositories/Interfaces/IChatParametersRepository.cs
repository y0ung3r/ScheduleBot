using ScheduleBot.Data.Models;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Interfaces.Repositories
{
    public interface IChatParametersRepository : IEntityFrameworkRepository<ChatParameters>
    {
        Task<ChatParameters> FindChatParameters(long chatId);
    }
}
