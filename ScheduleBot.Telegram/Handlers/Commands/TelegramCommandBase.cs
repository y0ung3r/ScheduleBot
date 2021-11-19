using BotFramework;
using BotFramework.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotFramework.Handlers.Common;
using ScheduleBot.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers.Commands
{
    public abstract class TelegramCommandBase : CommandHandlerBase<Update>
    {
        public ITelegramBotClient Client { get; }
        
        public TelegramCommandBase(ITelegramBotClient client)
        {
            Client = client;
        }
        
        private string[] ParseArguments(Message message, MessageEntity messageEntity)
        {
            var argumentsLine = message.Text.Substring(messageEntity.Length).TrimStart();

            return Regex.Split
            (
                input: argumentsLine,
                pattern: @"\s+"
            );
        }
        
        public override Task HandleAsync(Update request, RequestDelegate nextHandler)
        {
            var message = request.Message;
            var entity = message.Entities?.FirstOrDefault();

            return HandleAsync
            (
                message,
                ParseArguments
                (
                    message,
                    entity
                ),
                nextHandler
            );
        }

        public override bool CanHandle(Update request)
        {
            var botInfo = Client.GetMeAsync()
                                .GetAwaiter()
                                .GetResult();
            
            return request.IsCommand() && 
                   request.Message is Message message &&
                   message.IsContainsBotMention(botInfo) &&
                   this.TextIsCommandAlias(message.Text) &&
                   CanHandle(message);
        }

        public abstract Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler);

        public virtual bool CanHandle(Message message) => true;
    }
}
