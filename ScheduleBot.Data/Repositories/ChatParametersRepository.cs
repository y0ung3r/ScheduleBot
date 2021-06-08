using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Models;
using ScheduleBot.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Repositories
{
    public class ChatParametersRepository : EntityFrameworkRepositoryBase<BotContext, ChatParameters>, IChatParametersRepository
    {
        public ChatParametersRepository(BotContext context)
            : base(context)
        { }

        public async Task<ChatParameters> FindChatParameters(long chatId)
        {
            return await Models.FirstOrDefaultAsync(userSchedule => userSchedule.ChatId.Equals(chatId));
        } 
    }
}
