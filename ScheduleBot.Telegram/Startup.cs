using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ScheduleBot.Data;
using ScheduleBot.Data.Interfaces.Repositories;
using ScheduleBot.Data.UnitOfWorks;
using ScheduleBot.Interfaces;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Interfaces;
using System.Configuration;

namespace ScheduleBot.Telegram
{
    public class Startup : IStartup
    {
        public void Configure(IServiceCollection services)
        {
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
