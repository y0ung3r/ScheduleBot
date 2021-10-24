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
using ScheduleBot.Telegram.Handlers.Commands;
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
                        options.UseSqlite(BotConfiguration.ConnectionString);
                    })
                    .AddUnitOfWork<UnitOfWork>()
                    .AddRepositories()
                    .AddDomainServices()
                    .AddScheduleParser<ScheduleParser>()
                    .AddTelegramBotClient(BotConfiguration.ApiToken)
                    .AddTelegramBotExtensions()
                    .AddBotFramework()
                    .AddHandler<TelegramExceptionHandler>()
                    .AddHandler<StartCommand>()
                    .AddHandler<SettingsCommand>()
                    .AddHandler<BindCommand>()
                    .AddHandler<ScheduleCommand>()
                    .AddHandler<TomorrowCommand>()
                    .AddHandler<MissingUpdateHandler>();

            var serviceProvider = services.BuildServiceProvider();
            var application = serviceProvider.GetRequiredService<Application>();

            await application.RunAsync();
        }
    }
}
