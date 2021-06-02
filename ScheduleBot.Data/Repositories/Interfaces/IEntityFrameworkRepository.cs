using System.Threading.Tasks;

namespace ScheduleBot.Data.Interfaces.Repositories
{
    public interface IEntityFrameworkRepository<TModel>
        where TModel : class
    {
        Task<TModel> AddAsync(TModel model);

        void Remove(TModel model);
    }
}
