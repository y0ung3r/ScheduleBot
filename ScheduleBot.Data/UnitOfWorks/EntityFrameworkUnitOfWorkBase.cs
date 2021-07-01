using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Data.UnitOfWorks
{
    public abstract class EntityFrameworkUnitOfWorkBase<TContext> : IEntityFrameworkUnitOfWork
        where TContext : DbContext
    {
        protected TContext Context { get; }

        public EntityFrameworkUnitOfWorkBase(TContext context)
        {
            Context = context;
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
