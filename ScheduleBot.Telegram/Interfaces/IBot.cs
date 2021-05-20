using Telegram.Bot;

namespace ScheduleBot.Interfaces
{
    public interface IBot
    {
        TelegramBotClient Client { get; }

        void Run();
    }
}
