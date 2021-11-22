using BotFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleBot.Data;
using ScheduleBot.Data.Extensions;
using ScheduleBot.Data.UnitOfWorks;
using ScheduleBot.Domain.Extensions;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Extensions;
using ScheduleBot.Telegram.Configurations;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers;
using System.Threading.Tasks;
using ScheduleBot.Telegram.Handlers.Commands.Bind;
using ScheduleBot.Telegram.Handlers.Commands.Bind.StepHandlers;
using ScheduleBot.Telegram.Handlers.Commands.Settings;
using ScheduleBot.Telegram.Handlers.Commands.Start;

namespace ScheduleBot.Telegram
{
    public static class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication()
                    .AddLogging(builder =>
                    {
                        builder.ClearProviders()
                               .AddConsole();
                    })
                    .AddDbContext<BotContext>(options =>
                    {
                        options.UseSqlite(BotConfiguration.ConnectionString);
                    })
                    .AddUnitOfWork<UnitOfWork>()
                    .AddRepositories()
                    .AddDomainServices()
                    .AddScheduleParser<ScheduleParser>()
                    .AddTelegramBotClient(BotConfiguration.ApiToken)
                    .AddBotFramework()
                    .AddHandler<MissingUpdateHandler>()
                    .AddHandler<StartCommand>()
                    .AddHandler<BindCommand>()
                    .AddHandler<IncomingFacultyHandler>()
                    .AddHandler<IncomingGroupHandler>()
                    .AddHandler<TelegramExceptionHandler>()
                    .AddHandler<SettingsCommand>();
        }

        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var application = serviceProvider.GetRequiredService<Application>();

            await application.RunAsync();
        }
    }
}
