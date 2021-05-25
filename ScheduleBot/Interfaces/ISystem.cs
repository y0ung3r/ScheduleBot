using System.Threading.Tasks;

namespace ScheduleBot.Interfaces
{
    public interface ISystem
    {
        Task OnInitializeAsync();

        Task OnCommandReceivedAsync(object command);

        Task OnMessageReceivedAsync(object message);
    }

    public interface ISystem<TClient> : ISystem
    {
        TClient Client { get; }
    }
}
