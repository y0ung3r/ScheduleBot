using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.Repositories;

namespace ScheduleBot.Data
{
    public class BotUnitOfWork : EntityFrameworkUnitOfWork<BotContext>, IBotUnitOfWork
    {
        public IChatParametersRepository ChatParameters { get; } 

        public BotUnitOfWork(BotContext context)
            : base(context)
        {
            ChatParameters = new ChatParametersRepository(Context);
        }
    }
}
