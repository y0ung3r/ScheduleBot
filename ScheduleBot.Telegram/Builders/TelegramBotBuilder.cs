using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Builders;
using ScheduleBot.Interfaces;
using ScheduleBot.Telegram.Extensions;

namespace ScheduleBot.Telegram.Builders
{
    public class TelegramBotBuilder : BotBuilderBase
    {
        public override IBot Build()
        {
            return Services.AddTelegramClient(Token)
                           .AddTelegramBot(SystemTypes)
                           .BuildServiceProvider()
                           .GetRequiredService<IBot>();
        }
    }
}
