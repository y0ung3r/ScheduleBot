using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Extensions
{
    public static class TelegramTypesExtensions
    {
        public static string GetBotName(this User user) => $"{user.FirstName} {user.LastName}".Trim();

        public static long? GetChatId(this Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message.Chat.Id,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat.Id,
                UpdateType.EditedMessage => update.EditedMessage.Chat.Id,
                UpdateType.ChannelPost => update.ChannelPost.Chat.Id,
                UpdateType.EditedChannelPost => update.EditedChannelPost.Chat.Id,
                UpdateType.MyChatMember => update.MyChatMember.Chat.Id,
                UpdateType.ChatMember => update.ChatMember.Chat.Id,
                _ => default(long?)
            };
        }

        public static bool IsCommand(this Update update)
        {
            return update.Message?
                         .Entities?
                         .FirstOrDefault()?
                         .Type is MessageEntityType.BotCommand;
        }
    }
}
