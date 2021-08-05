using ScheduleBot.Extensions;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.Handlers;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Commands
{
    public abstract class TelegramCommandBase : TelegramHandlerBase, ICommandHandler
    {
        protected string[] ParseArguments(Message message, MessageEntity messageEntity)
        {
            var argumentsLine = message.Text.Substring(messageEntity.Length).TrimStart();

            return Regex.Split
            (
                input: argumentsLine,
                pattern: @"\s+"
            );
        }

        protected override Task HandleAsync(Update update, RequestDelegate nextHandler)
        {
            var message = update.Message;

            return HandleAsync
            (
                message,
                arguments: ParseArguments
                (
                    message,
                    messageEntity: message.Entities?.FirstOrDefault()
                ),
                nextHandler
            );
        }

        public bool CanHandle(object request)
        {
            return request is Update update && 
                   update.IsCommand() && 
                   this.IsCommandTextContains(update.Message.Text) && 
                   CanHandle(update.Message);
        }

        #region Abstract Methods

        protected abstract Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler);

        #endregion

        #region Virtual Methods

        protected virtual bool CanHandle(Message message) => true;

        #endregion
    }
}
