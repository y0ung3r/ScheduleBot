using System;

namespace ScheduleBot.Telegram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var botBuilder = new BotBuilder();

            var bot = botBuilder.Use<TelegramBot>()
                                .WithStartup<Startup>()
                                .Build();

            bot.Start();

            Console.WriteLine("Bot is running...");
            Console.ReadKey();

            bot.Stop();
        }
    }
}
