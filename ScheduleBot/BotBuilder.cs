using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Interfaces;
using System;

namespace ScheduleBot
{
    public class BotBuilder : IBotBuilder
    {
        private readonly IServiceCollection _services;
        private IStartup _startup;

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
            _startup = Activator.CreateInstance<TStartup>();
            _startup?.Configure(_services);

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