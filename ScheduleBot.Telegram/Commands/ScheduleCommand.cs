using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Extensions;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands
{
    [CommandText("/schedule")]
    public class ScheduleCommand : TelegramCommandBase
    {
        private readonly ILogger<ScheduleCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        private readonly ILongPollingService _longPollingService;
        private readonly IWeekDatesProvider _weekDatesProvider;

        public ScheduleCommand(ILogger<ScheduleCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
            IChatParametersService chatParametersService, ILongPollingService longPollingService, IWeekDatesProvider weekDatesProvider)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
            _longPollingService = longPollingService;
            _weekDatesProvider = weekDatesProvider;
        }

        protected override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var chatId = message.Chat.Id;

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            var messageToDelete = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите учебную неделю:",
                replyMarkup: DateTime.Today.ToWeekDatesKeyboard(_weekDatesProvider)
            );

            _longPollingService.RegisterStepHandler
            (
                chatId,
                HandleIncomingWeekAsync,
                messageToDelete
            );

            _logger?.LogInformation("Schedule command processed");
        }

        private async Task HandleIncomingWeekAsync(Update update, params object[] payload)
        {
            var payloadMessage = (Message)payload.First();
            var callbackQuery = update.CallbackQuery;

            if (callbackQuery is not null)
            {
                var chatId = callbackQuery.Message.Chat.Id;

                var weekDates = _weekDatesProvider.GetCurrentWeekDates
                (
                    DateTime.Parse(callbackQuery.Data)
                );

                var inlineKeyboard = weekDates.ToInlineKeyboard
                (
                    dateTime => dateTime.ToShortDateString(),
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
                    messageId: payloadMessage.MessageId
                );

                var messageToDelete = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Выберите дату:",
                    replyMarkup: inlineKeyboard
                );

                _longPollingService.RegisterStepHandler
                (
                    chatId,
                    HandleIncomingDateAsync,
                    messageToDelete
                );

                await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
            }
        }

        private async Task HandleIncomingDateAsync(Update update, params object[] payload)
        {
            var payloadMessage = (Message)payload.First();
            var callbackQuery = update.CallbackQuery;

            if (callbackQuery is not null)
            {
                var chatId = callbackQuery.Message.Chat.Id;

                if (DateTime.TryParse(callbackQuery.Data, out var dateTime))
                {
                    var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

                    await _client.SendChatActionAsync
                    (
                        chatId,
                        chatAction: ChatAction.Typing
                    );

                    await _client.DeleteMessageAsync
                    (
                        chatId,
                        messageId: payloadMessage.MessageId
                    );

                    if (chatParameters is not null)
                    {
                        var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                        var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
                        var html = studyDay.ToHTML();

                        var essageToDelete = await _client.SendTextMessageAsync
                        (
                            chatId,
                            text: html,
                            parseMode: ParseMode.Html,
                            replyMarkup: dateTime.ToNavigationKeyboard()
                        );

                        _longPollingService.RegisterStepHandler
                        (
                            chatId,
                            HandleIncomingDateAsync,
                            essageToDelete
                        );
                    }
                    else
                    {
                        await _client.SendTextMessageAsync
                        (
                            chatId,
                            text: "Вы не настроили бота, чтобы использовать этот функционал. Используйте /bind, чтобы начать работу",
                            parseMode: ParseMode.Html
                        );
                    }
                }

                await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
            }
        }
    }
}
