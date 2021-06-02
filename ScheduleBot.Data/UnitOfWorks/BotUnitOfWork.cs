using ScheduleBot.Data.Interfaces.Repositories;
using ScheduleBot.Data.Repositories;

namespace ScheduleBot.Data.UnitOfWorks
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
