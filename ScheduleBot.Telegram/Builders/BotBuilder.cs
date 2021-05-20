using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;

namespace ScheduleBot.Builders
{
    public class BotBuilder : IBotBuilder
    {
        private readonly IServiceCollection _services;
        private readonly ICollection<Type> _systemTypes;
        private string _token;

        public BotBuilder()
        {
            _services = new ServiceCollection();
            _systemTypes = new List<Type>();
        }

        public IBotBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices is null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            configureServices.Invoke(_services);

            return this;
        }

        public IBotBuilder SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            _token = token;

            return this;
        }

        public IBotBuilder UseSystem<TSystem>()
            where TSystem : ISystem
        {
            var systemType = typeof(TSystem);
            _services.TryAddScoped(systemType);

            if (systemType is null)
            {
                throw new InvalidOperationException($"System \"{systemType}\" is not registered as a dependency");
            }

            _systemTypes.Add(systemType);

            return this;
        }

        public IBot Build()
        {
            _services.AddBot
            (
                _token,
                _systemTypes
            );

            return _services.BuildServiceProvider()
                            .GetRequiredService<IBot>();
        }
    }
}
