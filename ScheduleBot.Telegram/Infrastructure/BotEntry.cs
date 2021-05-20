using ScheduleBot.Builders;
using ScheduleBot.Interfaces;

namespace ScheduleBot.Infrastructure
{
    public static class BotEntry
    {
        public static IBotBuilder CreateDefaultBotBuilder() => new BotBuilder();
    }
}
