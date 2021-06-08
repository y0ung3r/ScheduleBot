using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ScheduleBot.Data;
using ScheduleBot.Data.UnitOfWorks;
using ScheduleBot.Data.UnitOfWorks.Interfaces;
using ScheduleBot.Interfaces;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using System.Configuration;

namespace ScheduleBot.Telegram
{
    public class Startup : IStartup
    {
        public void Configure(IServiceCollection services)
        {
            var token = ConfigurationManager.AppSettings["token"];
            services.UseTelegramBotClient(token);

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
            });

            services.AddDbContext<BotContext>(options =>
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ScheduleBot"].ConnectionString;
                options.UseSqlServer(connectionString);
            });

            services.TryAddScoped<IBotUnitOfWork, BotUnitOfWork>();
            services.TryAddSingleton<IScheduleParser, ScheduleParser>();
        }
    }
}
