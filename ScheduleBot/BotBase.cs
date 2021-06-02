using ScheduleBot.Interfaces;

namespace ScheduleBot
{
    public abstract class BotBase : IBot, IRunnable
    {
        public abstract void Run();
    }
}
