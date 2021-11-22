﻿using BotFramework;
using BotFramework.Interfaces;
using ScheduleBot.Telegram.Handlers;
using ScheduleBot.Telegram.Interfaces;
using System;
using System.Threading.Tasks;
using BotFramework.Extensions;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers.Commands.Bind;
using ScheduleBot.Telegram.Handlers.Commands.Bind.StepHandlers;
using ScheduleBot.Telegram.Handlers.Commands.Settings;
using ScheduleBot.Telegram.Handlers.Commands.Start;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram
{
    public class Application
    {
        private readonly Func<RequestDelegate, ITelegramBot> _botFactory;
        private readonly IBranchBuilder _branchBuilder;

        public Application(Func<RequestDelegate, ITelegramBot> botFactory, IBranchBuilder branchBuilder)
        {
            _botFactory = botFactory;
            _branchBuilder = branchBuilder;

            _branchBuilder.UseHandler<TelegramExceptionHandler>()
                          .UseCommand<StartCommand>()
                          .UseStepsFor<BindCommand>
                          (
                              stepsBuilder =>
                              { 
                                  stepsBuilder.UseStepHandler<IncomingFacultyHandler>()
                                              .UseStepHandler<IncomingGroupHandler>();
                              },
                              request => (request as Update).GetChatId()
                          )
                          .UseCommand<SettingsCommand>()
                          .UseHandler<MissingUpdateHandler>();
        }

        public async Task RunAsync()
        {
            var branch = _branchBuilder.Build();
            var bot = _botFactory(branch);
            var botInfo = await bot.GetBotInfoAsync();

            await bot.RunAsync();

            Console.Title = botInfo.GetBotName();
            Console.ReadKey();

            bot.Stop();
        }
    }
}
