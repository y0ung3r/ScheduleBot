using ScheduleBot.Data.Repositories.Interfaces;
using ScheduleBot.Data.UnitOfWorks.Interfaces;

namespace ScheduleBot.Data.UnitOfWorks
{
    public class UnitOfWork : EntityFrameworkUnitOfWorkBase<BotContext>, IUnitOfWork
    {
        public IChatParametersRepository ChatParameters { get; } 

        public UnitOfWork(BotContext context, IChatParametersRepository chatParameters)
            : base(context)
        {
            ChatParameters = chatParameters;
        }
    }
}
