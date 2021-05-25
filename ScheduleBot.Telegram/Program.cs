﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ScheduleBot.Data;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.UnitOfWorks;
using ScheduleBot.Parser;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Builders;
using ScheduleBot.Telegram.Systems;
using System.Configuration;

namespace ScheduleBot.Telegram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var botBuilder = new TelegramBotBuilder();

            botBuilder.ConfigureServices(services =>
                      {
                          services.AddLogging(loggingBuilder =>
                          {
                              loggingBuilder.ClearProviders();
                              loggingBuilder.AddConsole();
                          });

                          services.AddDbContext<BotContext>(options =>
                          {
                              options.UseSqlServer
                              (
                                  ConfigurationManager.ConnectionStrings["ScheduleBot"].ConnectionString
                              );
                          });

                          services.TryAddScoped<IBotUnitOfWork, BotUnitOfWork>();
                          services.TryAddSingleton<IScheduleParser, ScheduleParser>();
                      })
                      .SetToken(ConfigurationManager.AppSettings["token"])
                      .UseSystem<MainSystem>()
                      .UseSystem<FetchSettingsSystem>()
                      .UseSystem<BindSystem>()
                      .UseSystem<FetchScheduleSystem>()
                      .Build()
                      .Run();
        }
    }
}
