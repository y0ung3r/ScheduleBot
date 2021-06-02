using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.UnitOfWorks.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Data.UnitOfWorks
{
    public class EntityFrameworkUnitOfWork<TContext> : IEntityFrameworkUnitOfWork
        where TContext : DbContext
    {
        protected TContext Context { get; }

        public EntityFrameworkUnitOfWork(TContext context)
        {
            Context = context;
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
