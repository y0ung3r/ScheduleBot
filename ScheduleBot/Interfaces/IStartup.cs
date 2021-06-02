using Microsoft.Extensions.DependencyInjection;

namespace ScheduleBot.Interfaces
{
    public interface IStartup
    {
        void Configure(IServiceCollection services);
    }
}
