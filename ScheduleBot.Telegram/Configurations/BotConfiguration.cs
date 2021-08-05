using System.Configuration;

namespace ScheduleBot.Telegram.Configurations
{
    public static class BotConfiguration
    {
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["ScheduleBot"].ConnectionString;

        public static string ApiToken { get; } = ConfigurationManager.AppSettings["token"];
    }
}
