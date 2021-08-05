using System.Threading.Tasks;

namespace ScheduleBot.Data.Repositories.Interfaces
{
    public interface IEntityFrameworkRepository<TModel>
        where TModel : class
    {
        Task<TModel> AddAsync(TModel model);

        void Remove(TModel model);
    }
}
