using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Commands;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Interfaces;
using ScheduleBot.States;
using ScheduleBot.States.Interfaces;
using System;
using System.Collections.Generic;

namespace ScheduleBot
{
    public class BotBuilder : IBotBuilder
    {
        private readonly IServiceCollection _services;
        private readonly ICommandManager _commandManager;
        private readonly ICollection<Type> _commandTypes;

        public BotBuilder()
        {
            _services = new ServiceCollection();

            _commandManager = new CommandManager();
            _services.TryAddSingleton(_commandManager);
        }

        public IBotBuilder Use<TBot>()
            where TBot : IBot
        {
            _services.TryAddSingleton
            (
                typeof(IBot),
                typeof(TBot)
            );

            return this;
        }

        public IBotBuilder WithStartup<TStartup>()
            where TStartup : IStartup
        {
            var startup = Activator.CreateInstance<TStartup>();

            _services.TryAddSingleton<IStartup>(startup);
            _services.TryAddSingleton<IStateProvider, StateProvider>();

            startup?.Configure(_services);

            return this;
        }

        public IBotBuilder WithCommand<TBotCommand>()
            where TBotCommand : IBotCommand
        {
            var commandType = typeof(TBotCommand);

            _services.TryAddScoped
            (
                commandType,
                commandType
            );

            _commandTypes.Add(commandType);

            return this;
        }

        public IBot Build()
        {
            var serviceProvider = _services.BuildServiceProvider();

            foreach (var commandType in _commandTypes)
            {
                _commandManager.RegisterCommand
                (
                    (IBotCommand)serviceProvider.GetRequiredService(commandType)
                );
            }

            return serviceProvider.GetRequiredService<IBot>();
        }
    }
}