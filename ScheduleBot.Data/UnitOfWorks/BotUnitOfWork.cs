using ScheduleBot.Data.Repositories;
using ScheduleBot.Data.Repositories.Interfaces;
using ScheduleBot.Data.UnitOfWorks.Interfaces;

namespace ScheduleBot.Data.UnitOfWorks
{
    public class BotUnitOfWork : EntityFrameworkUnitOfWorkBase<BotContext>, IBotUnitOfWork
    {
        public IChatParametersRepository ChatParameters { get; } 

        public BotUnitOfWork(BotContext context)
            : base(context)
        {
            ChatParameters = new ChatParametersRepository(Context);
        }
    }
}
