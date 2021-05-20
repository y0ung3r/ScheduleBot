namespace ScheduleBot.Interfaces
{
    public interface IBotConfigurator
    {
        void SetToken(string token);

        void UseSystem<TSystem>() where TSystem : ISystem;
    }
}
