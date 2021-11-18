using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Interfaces
{
    public interface ITelegramBot
    {
        Task RunAsync();

        Task<User> GetBotInfoAsync();

        void Stop();
    }
}
