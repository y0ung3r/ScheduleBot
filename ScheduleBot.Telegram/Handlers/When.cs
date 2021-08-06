using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers
{
    public static class When
    {
        public static bool MessageReceived(object request) => request is Update update && update.Message is not null;

        public static bool CallbackQueryReceived(object request) => request is Update update && update.CallbackQuery is not null;
    }
}
