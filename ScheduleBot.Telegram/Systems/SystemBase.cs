using ScheduleBot.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Systems
{
    public abstract class SystemBase : ISystem
    {
        protected Bot Bot { get; private set; }

        public void Initialize(IBot bot)
        {
            if (bot is null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            if (Bot != null)
            {
                throw new InvalidOperationException("Bot has already attached to this system");
            }

            Bot = bot as Bot;
        }

        public virtual async Task OnStartupAsync()
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnUpdateReceivedAsync(Update update)
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnCommandReceivedAsync(Message message)
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnMessageReceivedAsync(Message message)
        {
            await Task.CompletedTask;
        }
    }
}
