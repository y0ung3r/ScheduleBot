using System.Threading.Tasks;
using BotFramework;
using BotFramework.Attributes;
using Microsoft.Extensions.Logging;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands.Bind
{
    [CommandAliases("/bind")]
    internal sealed class BindCommand : TelegramCommandBase
    {
        private readonly ILogger<BindCommand> _logger;
        private readonly IScheduleParser _scheduleParser;
        
        public BindCommand(ITelegramBotClient client, ILogger<BindCommand> logger, IScheduleParser scheduleParser)
            : base(client)
        {
            _logger = logger;
            _scheduleParser = scheduleParser;
        }
        
        public override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var chatId = message.Chat.Id;

            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var inlineKeyboard = faculties.ToInlineKeyboard
            (
                faculty => faculty.TitleAbbreviation,
                columnsCount: 2
            );

            await Client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            await Client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: inlineKeyboard
            );

            _logger?.LogInformation("Bind command is running...");
        }
    }
}