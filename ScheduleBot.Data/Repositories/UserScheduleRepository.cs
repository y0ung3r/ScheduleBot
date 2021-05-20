using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.Models;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Repositories
{
    public class UserScheduleRepository : EntityFrameworkRepository<BotContext, UserSchedule>, IUserScheduleRepository
    {
        public UserScheduleRepository(BotContext context)
            : base(context)
        { }

        public async Task<UserSchedule> FindUserSchedule(long chatId)
        {
            return await Models.FirstOrDefaultAsync(userSchedule => userSchedule.ChatId.Equals(chatId));
        } 
    }
}
