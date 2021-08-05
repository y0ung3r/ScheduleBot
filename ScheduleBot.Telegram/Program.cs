using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleBot.Data;
using ScheduleBot.Data.Extensions;
using ScheduleBot.Data.UnitOfWorks;
using ScheduleBot.Domain.Extensions;
using ScheduleBot.Extensions;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Extensions;
using ScheduleBot.Telegram.Configurations;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers;
using ScheduleBot.Telegram.Commands;
using System.Threading.Tasks;

namespace ScheduleBot.Telegram
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddApplication()
                    .AddLogging(builder =>
                    {
                        builder.ClearProviders()
                               .AddConsole();
                    })
                    .AddDbContext<BotContext>(options =>
                    {
                        options.UseSqlServer(BotConfiguration.ConnectionString);
                    })
                    .AddUnitOfWork<UnitOfWork>()
                    .AddRepositories()
                    .AddDomainServices()
                    .AddScheduleParser<ScheduleParser>()
                    .AddTelegramBotClient(BotConfiguration.ApiToken)
                    .AddTelegramLongPolling()
                    .AddBot<TelegramScheduleBot>()
                    .AddHandler<StartCommand>()
                    .AddHandler<SettingsCommand>()
                    .AddHandler<BindCommand>()
                    .AddHandler<ScheduleCommand>()
                    .AddHandler<TomorrowCommand>()
                    .AddHandler<TelegramExceptionHandler>();

            var serviceProvider = services.BuildServiceProvider();
            var application = serviceProvider.GetRequiredService<Application>();

            await application.RunAsync();
        }
    }
}
