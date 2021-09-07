using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers;
using ScheduleBot.Telegram.Handlers.Commands;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram
{
    public class Application
    {
        private readonly Func<RequestDelegate, IBot> _botFactory;
        private readonly IBranchBuilder _branchBuilder;

        public Application(Func<RequestDelegate, IBot> botFactory, IBranchBuilder branchBuilder)
        {
            _botFactory = botFactory;
            _branchBuilder = branchBuilder;
        }

        private void ConfigureBranchBuilder()
        {
            _branchBuilder.UseHandler<TelegramExceptionHandler>()
                          .UseCommand<StartCommand>()
                          .UseCommand<SettingsCommand>()
                          .UseCommand<BindCommand>()
                          .UseCommand<ScheduleCommand>()
                          .UseCommand<TomorrowCommand>()
                          .UseHandler<MissingUpdateHandler>();
        }

        private TelegramScheduleBot CreateTelegramScheduleBot()
        {
            ConfigureBranchBuilder();

            return (TelegramScheduleBot)_botFactory
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
