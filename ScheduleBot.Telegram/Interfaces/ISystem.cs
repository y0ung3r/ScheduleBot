using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Interfaces
{
    public interface ISystem
    {
        Task OnInlineQueryReceivedAsync(ITelegramBotClient client, InlineQuery inlineQuery);

        Task OnCallbackQueryReceivedAsync(ITelegramBotClient client, CallbackQuery callbackQuery);

        Task OnCommandReceivedAsync(ITelegramBotClient client, Message command);

        Task OnMessageReceivedAsync(ITelegramBotClient client, Message message);

        Task OnEditedMessageReceivedAsync(ITelegramBotClient client, Message message);

        Task OnUnknownUpdateReceivedAsync(ITelegramBotClient client, Update update);
    }
}
