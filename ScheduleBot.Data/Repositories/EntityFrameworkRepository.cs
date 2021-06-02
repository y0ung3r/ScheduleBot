using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Interfaces.Repositories;
using System.Threading.Tasks;

namespace ScheduleBot.Data.Repositories
{
    public class EntityFrameworkRepository<TContext, TModel> : IEntityFrameworkRepository<TModel>
        where TContext : DbContext
        where TModel : class
    {
        protected TContext Context { get; }
        protected DbSet<TModel> Models => Context.Set<TModel>();

        public EntityFrameworkRepository(TContext context)
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
