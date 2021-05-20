using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Interfaces
{
    public interface ISystem
    {
        void Initialize(IBot bot);

        Task OnStartupAsync();

        Task OnCommandReceivedAsync(Message message);

        Task OnMessageReceivedAsync(Message message);
    }
}
