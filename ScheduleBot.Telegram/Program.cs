using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Data;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Infrastructure;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser;
using ScheduleBot.Systems;
using System.Configuration;

namespace ScheduleBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BotEntry.CreateDefaultBotBuilder()
                    .ConfigureServices(services =>
                    {
                        services.AddDbContext<BotContext>(options =>
                        {
                            if (!options.IsConfigured)
                            {
                                options.UseSqlServer(ConfigurationManager.ConnectionStrings["ScheduleBot"].ConnectionString);
                            }
                        });

                        services.TryAddScoped<IBotUnitOfWork, BotUnitOfWork>();
                        services.TryAddSingleton<IScheduleParser, ScheduleParser>();
                    })
                    .SetToken(ConfigurationManager.AppSettings["token"])
                    .UseSystem<SignUpSystem>()
                    .UseSystem<FetchScheduleSystem>()
                    .Build()
                    .Run();
        }
    }
}
