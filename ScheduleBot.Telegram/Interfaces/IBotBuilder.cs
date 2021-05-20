using Microsoft.Extensions.DependencyInjection;
using System;

namespace ScheduleBot.Interfaces
{
    public interface IBotBuilder
    {
        IBotBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IBotBuilder SetToken(string token);

        IBotBuilder UseSystem<TSystem>() where TSystem : ISystem;

        IBot Build();
    }
}
