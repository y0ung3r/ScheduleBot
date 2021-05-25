using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;

namespace ScheduleBot.Builders
{
    public abstract class BotBuilderBase : IBotBuilder
    {
        protected IServiceCollection Services { get; }
        protected ICollection<Type> SystemTypes { get; }
        protected string Token { get; private set; }

        public BotBuilderBase()
        {
            Services = new ServiceCollection();
            SystemTypes = new List<Type>();
        }

        public IBotBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices is null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            configureServices.Invoke(Services);

            return this;
        }

        public IBotBuilder SetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;

            return this;
        }

        public IBotBuilder UseSystem<TSystem>()
            where TSystem : ISystem
        {
            var systemType = typeof(TSystem);
            Services.TryAddScoped(systemType);

            if (systemType is null)
            {
                throw new InvalidOperationException($"System \"{systemType}\" is not registered as a dependency");
            }

            SystemTypes.Add(systemType);

            return this;
        }

        public abstract IBot Build();
    }
}
