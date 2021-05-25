using ScheduleBot.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Systems
{
    public abstract class TelegramSystemBase : ISystem<ITelegramBotClient>
    {
        public ITelegramBotClient Client { get; private set; }

        protected virtual Task OnCommandReceivedAsync(Message command)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnMessageReceivedAsync(Message message)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnInitializeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnInlineQueryReceivedAsync(InlineQuery inlineQuery)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
        {
            return Task.CompletedTask;
        }

        public Task OnCommandReceivedAsync(object command)
        {
            return OnCommandReceivedAsync(command as Message);
        }

        public Task OnMessageReceivedAsync(object message)
        {
            return OnCommandReceivedAsync(message as Message);
        }

        public virtual Task OnEditedMessageReceivedAsync(Message message)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnknownUpdateReceivedAsync(Update update)
        {
            return Task.CompletedTask;
        }
    }
}
