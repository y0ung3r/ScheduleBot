using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Builders;
using ScheduleBot.Data;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Systems;
using System.Configuration;

namespace ScheduleBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var botBuilder = new BotBuilder();

            botBuilder.ConfigureServices(services =>
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
                      .UseSystem<MainSystem>()
                      .UseSystem<FetchSettingsSystem>()
                      .UseSystem<SetupSystem>()
                      .UseSystem<FetchScheduleSystem>()
                      .Build()
                      .Run();
        }
    }
}
