namespace ScheduleBot.Telegram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new BotBuilder().Use<TelegramBot>()
                            .WithStartup<Startup>()
                            .Build()
                            .Run();
        }
    }
}
