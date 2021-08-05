using ScheduleBot.Domain.DTO;
using System.Threading.Tasks;

namespace ScheduleBot.Domain.Interfaces
{
    public interface IChatParametersService
    {
        Task<ChatParametersDTO> GetChatParametersAsync(long chatId);

        Task SaveChatParametersAsync(long chatId, int facultyId, int groupId, int groupTypeId);
    }
}
