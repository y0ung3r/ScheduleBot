using ScheduleBot.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Systems
{
    public abstract class SystemBase : ISystem
    {
        public virtual Task OnInlineQueryReceivedAsync(ITelegramBotClient client, InlineQuery inlineQuery)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCallbackQueryReceivedAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCommandReceivedAsync(ITelegramBotClient client, Message command)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnMessageReceivedAsync(ITelegramBotClient client, Message message)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnEditedMessageReceivedAsync(ITelegramBotClient client, Message message)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnknownUpdateReceivedAsync(ITelegramBotClient client, Update update)
        {
            return Task.CompletedTask;
        }
    }
}
