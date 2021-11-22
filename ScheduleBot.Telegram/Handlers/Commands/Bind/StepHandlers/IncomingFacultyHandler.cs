using System.Linq;
using System.Threading.Tasks;
using BotFramework.Handlers.StepHandlers;
using Microsoft.Extensions.Logging;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands.Bind.StepHandlers
{
    internal sealed class IncomingFacultyHandler : StepHandlerBase<Update, Update>
    {
        private readonly ILogger<IncomingFacultyHandler> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        
        public IncomingFacultyHandler(ILogger<IncomingFacultyHandler> logger, ITelegramBotClient client, IScheduleParser scheduleParser)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
        }
        
        public override async Task HandleAsync(Update request, Update response)
        {
            var callbackQuery = response.CallbackQuery;
            var message = callbackQuery.Message;
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            
            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var facultyAbbreviation = callbackQuery.Data;
            var faculty = faculties.FirstOrDefault(faculty => faculty.TitleAbbreviation.Equals(facultyAbbreviation));

            if (faculty is not null)
            {
                var groups = await _scheduleParser.ParseGroupsAsync(faculty.Id);
                var inlineKeyboard = groups.ToInlineKeyboard
                (
                    group => group.Title,
                    columnsCount: 3
                );

                await _client.SendChatActionAsync(chatId, ChatAction.Typing);
                await _client.DeleteMessageAsync(chatId, messageId);

                await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Теперь выберите группу:",
                    replyMarkup: inlineKeyboard
                );
            }

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        public override bool CanHandle(Update request, Update response)
        {
            return request is not null && response is not null;
        }
    }
}