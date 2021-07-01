using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Repositories
{
    public abstract class EntityFrameworkRepositoryBase<TContext, TModel> : IEntityFrameworkRepository<TModel>
        where TContext : DbContext
        where TModel : class
    {
        protected TContext Context { get; }
        protected DbSet<TModel> Models => Context.Set<TModel>();

        public EntityFrameworkRepositoryBase(TContext context)
        {
            Context = context;
        }

        public async Task<TModel> AddAsync(TModel model)
        {
            await Models.AddAsync(model);

            return model;
        }

        public void Remove(TModel model)
        {
            Models.Remove(model);
        }
    }
}
