using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands
{
    [CommandText("/bind")]
    public class BindCommand : TelegramCommandBase
    {
        private class BindPayload
        {
            public Message PreviousMessage { get; set; }

            public int? FacultyId { get; set; }
        }

        private readonly ILogger<BindCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        private readonly ILongPollingService _longPollingService;

        public BindCommand(ILogger<BindCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser, 
            IChatParametersService chatParametersService, ILongPollingService longPollingService)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
            _longPollingService = longPollingService;
        }

        protected override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var chatId = message.Chat.Id;
            
            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var inlineKeyboard = faculties.ToInlineKeyboard
            (
                faculty => faculty.TitleAbbreviation,
                columnsCount: 2
            );

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            var lastMessage = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: inlineKeyboard
            );

            _longPollingService.RegisterStepHandler
            (
                chatId, 
                HandleIncomingFacultyAsync,
                new BindPayload
                {
                    PreviousMessage = lastMessage
                }
            );

            _logger?.LogInformation("Bind command executed");
        }

        private async Task HandleIncomingFacultyAsync(Update update, object payload)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var bindPayload = (BindPayload)payload;
            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var facultyAbbreviation = callbackQuery.Data;
            var faculty = faculties.FirstOrDefault(faculty => faculty.TitleAbbreviation.Equals(facultyAbbreviation));
            var groups = await _scheduleParser.ParseGroupsAsync(faculty.Id);

            var inlineKeyboard = groups.ToInlineKeyboard
            (
                group => group.Title,
                columnsCount: 3
            );

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            await _client.DeleteMessageAsync
            (
                chatId, 
                messageId: bindPayload.PreviousMessage.MessageId
            );

            var lastMessage = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Теперь выберите группу:",
                replyMarkup: inlineKeyboard
            );

            _longPollingService.RegisterStepHandler
            (
                chatId, 
                HandleIncomingGroupAsync, 
                new BindPayload
                {
                    PreviousMessage = lastMessage,
                    FacultyId = faculty.Id
                }
            );

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async Task HandleIncomingGroupAsync(Update update, object payload)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var bindPayload = (BindPayload)payload;
            var facultyId = (int)bindPayload.FacultyId;
            var groups = await _scheduleParser.ParseGroupsAsync(facultyId);
            var groupTitle = callbackQuery.Data;
            var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            await _chatParametersService.SaveChatParametersAsync
            (
                chatId, 
                facultyId, 
                group.Id, 
                group.TypeId
            );

            await _client.DeleteMessageAsync
            (
                chatId, 
                messageId: bindPayload.PreviousMessage.MessageId
            );

            await _client.SendTextMessageAsync
            (
                chatId,
                text: "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
            );

            _logger?.LogInformation("Bind command processed");

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }
    }
}
