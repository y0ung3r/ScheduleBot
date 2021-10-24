using BotFramework;
using BotFramework.Interfaces;
using BotFramework.Extensions;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers;
using ScheduleBot.Telegram.Handlers.Commands;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram
{
    public class Application
    {
        private readonly IBotFactory _botFactory;
        private readonly IBranchBuilder _branchBuilder;

        public Application(IBotFactory botFactory, IBranchBuilder branchBuilder)
        {
            _botFactory = botFactory;
            _branchBuilder = branchBuilder;
        }

        private TelegramScheduleBot CreateTelegramScheduleBot()
        {
            _branchBuilder.UseHandler<TelegramExceptionHandler>()
                          .UseCommand<StartCommand>()
                          .UseCommand<SettingsCommand>()
                          .UseCommand<BindCommand>()
                          .UseCommand<ScheduleCommand>()
                          .UseCommand<TomorrowCommand>()
                          .UseHandler<MissingUpdateHandler>();

            return _botFactory.Create<TelegramScheduleBot>
            (
                _branchBuilder.Build()
            );
        }

        public async Task RunAsync()
        {
            var bot = CreateTelegramScheduleBot();
            var botInfo = await bot.GetBotInfoAsync();

            bot.Run();

            Console.Title = botInfo.GetBotName();
            Console.ReadKey();

            bot.Stop();
        }
    }
}
