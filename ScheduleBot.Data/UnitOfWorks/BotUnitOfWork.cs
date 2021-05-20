using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.Repositories;

namespace ScheduleBot.Data
{
    public class BotUnitOfWork : EntityFrameworkUnitOfWork<BotContext>, IBotUnitOfWork
    {
        public IUserScheduleRepository UserSchedules { get; } 

        public BotUnitOfWork(BotContext context)
            : base(context)
        {
            UserSchedules = new UserScheduleRepository(Context);
        }
    }
}
