using BotFramework;
using BotFramework.Handlers.Interfaces;
using BotFramework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Telegram.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers.Commands
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

        public bool CanHandle(IServiceProvider serviceProvider, object request)
        {
            var client = serviceProvider.GetRequiredService<ITelegramBotClient>();
            var botInfo = client.GetMeAsync()
                                .GetAwaiter()
                                .GetResult();

            return request is Update update && 
                   update.IsCommand() && 
                   update.Message is Message message &&
                   message.IsContainsBotMention(botInfo) &&
                   this.TextIsCommandAlias(message.Text) &&
                   CanHandle(message);
        }

        #region Abstract Methods

        protected abstract Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler);

        #endregion

        #region Virtual Methods

        protected virtual bool CanHandle(Message message) => true;

        #endregion
    }
}
