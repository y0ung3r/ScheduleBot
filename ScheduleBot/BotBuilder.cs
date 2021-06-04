using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Commands;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Interfaces;
using System;

namespace ScheduleBot
{
    public class BotBuilder : IBotBuilder
    {
        private readonly IServiceCollection _services;

        public BotBuilder()
        {
            _services = new ServiceCollection();
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
            _services.TryAddSingleton<ICommandManager, CommandManager>();

            startup?.Configure(_services);

            return this;
        }

        public IBotBuilder WithCommand<TCommand>()
            where TCommand : class
        {
            _services.TryAddScoped<TCommand, TCommand>();

            return this;
        }

        public IRunnable Build()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var bot = serviceProvider.GetRequiredService<IBot>();

            return (IRunnable)bot;
        }
    }
}