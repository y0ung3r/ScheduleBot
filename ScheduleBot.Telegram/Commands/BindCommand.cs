using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.StepHandler.Interfaces;
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
        private readonly ILogger<BindCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        private readonly ICallbackQueryListener _callbackQueryListener;

        public BindCommand(ILogger<BindCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
            IChatParametersService chatParametersService, ICallbackQueryListener callbackQueryListener)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
            _callbackQueryListener = callbackQueryListener;
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

            var request = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: inlineKeyboard
            );

            _callbackQueryListener.RegisterRequest
            (
                request,
                HandleIncomingFacultyAsync
            );

            _logger?.LogInformation("Bind command processed");
        }

        private async Task HandleIncomingFacultyAsync(Message request, CallbackQuery response, params object[] payload)
        {
            var chatId = response.Message.Chat.Id;

            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var facultyAbbreviation = response.Data;
            var faculty = faculties.FirstOrDefault(faculty => faculty.TitleAbbreviation.Equals(facultyAbbreviation));

            if (faculty is not null)
            {
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
                    messageId: request.MessageId
                );

                var nextRequest = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Теперь выберите группу:",
                    replyMarkup: inlineKeyboard
                );

                _callbackQueryListener.RegisterRequest
                (
                    nextRequest,
                    HandleIncomingGroupAsync,
                    faculty.Id
                );
            }

            await _client.AnswerCallbackQueryAsync(response.Id);
        }

        private async Task HandleIncomingGroupAsync(Message request, CallbackQuery response, params object[] payload)
        {
            var facultyId = (int)payload.First();
            var chatId = response.Message.Chat.Id;

            var groups = await _scheduleParser.ParseGroupsAsync(facultyId);
            var groupTitle = response.Data;
            var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

            if (group is not null)
            {
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
                    messageId: request.MessageId
                );

                await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
                );
            }

            await _client.AnswerCallbackQueryAsync(response.Id);

        }
    }
}
