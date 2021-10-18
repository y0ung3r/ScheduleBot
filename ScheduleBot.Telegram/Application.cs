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
        private readonly Func<RequestDelegate, IBot> _botFactoryMethod;
        private readonly IBranchBuilder _branchBuilder;

        public Application(Func<RequestDelegate, IBot> botFactoryMethod, IBranchBuilder branchBuilder)
        {
            _botFactoryMethod = botFactoryMethod;
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

            return (TelegramScheduleBot)_botFactoryMethod
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
