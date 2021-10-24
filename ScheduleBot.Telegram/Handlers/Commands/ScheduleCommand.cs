using BotFramework;
using BotFramework.Attributes;
using Microsoft.Extensions.Logging;
using ScheduleBot.Domain.Extensions;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.StepHandler.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands
{
    [CommandText("/schedule")]
    public class ScheduleCommand : TelegramCommandBase
    {
        private readonly ILogger<ScheduleCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        private readonly IStepRequestStorage _stepRequestStorage;
        private readonly IWeekDatesProvider _weekDatesProvider;

        public ScheduleCommand(ILogger<ScheduleCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
            IChatParametersService chatParametersService, IStepRequestStorage stepRequestStorage, IWeekDatesProvider weekDatesProvider)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
            _stepRequestStorage = stepRequestStorage;
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

            var incomingDateTime = arguments.FirstOrDefault();

            if (DateTime.TryParse(incomingDateTime, out DateTime dateTime))
            {
                if (dateTime.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    await _client.SendTextMessageAsync
                    (
                        chatId,
                        text: $"{dateTime.ToShortDateString()} - выходной день. Используйте /schedule, чтобы получить расписание на следующую неделю"
                    );
                }
                else
                {
                    var groupTitle = arguments.LastOrDefault();

                    if (arguments.Length > 1 && !string.IsNullOrWhiteSpace(groupTitle))
                    {
                        await SendScheduleAsync(chatId, dateTime, groupTitle);
                    }
                    else
                    {
                        await SendScheduleAsync(chatId, dateTime);
                    }
                }
            }
            else
            {
                var request = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: "Выберите учебную неделю:",
                    replyMarkup: DateTime.Today.ToWeekDatesKeyboard(_weekDatesProvider)
                );

                _stepRequestStorage.PushRequest
                (
                    request,
                    HandleIncomingWeekAsync
                );
            }

            _logger?.LogInformation("Schedule command processed");
        }

        private async Task HandleIncomingWeekAsync(Message request, Update response, params object[] payload)
        {
            var callbackQuery = response.CallbackQuery;
            var chatId = request.Chat.Id;

            var weekDates = _weekDatesProvider.GetCurrentWeekDates
            (
                DateTime.Parse(callbackQuery.Data)
            );

            var inlineKeyboard = weekDates.ToInlineKeyboard
            (
                dateTime => $"{dateTime:dd.MM (ddd)}",
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
                text: "Выберите дату:",
                replyMarkup: inlineKeyboard
            );

            _stepRequestStorage.PushRequest
            (
                nextRequest,
                HandleIncomingDateAsync
            );

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async Task HandleIncomingDateAsync(Message request, Update response, params object[] payload)
        {
            var callbackQuery = response.CallbackQuery;
            var chatId = request.Chat.Id;
            var data = callbackQuery.Data.Split(" ").FirstOrDefault();

            if (DateTime.TryParse(data, out var dateTime))
            {
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

                if (payload.FirstOrDefault() is string groupTitle)
                {
                    await SendScheduleAsync(chatId, dateTime, groupTitle);
                }
                else
                {
                    await SendScheduleAsync(chatId, dateTime);
                }
            }

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async Task SendScheduleAsync(long chatId, DateTime dateTime)
        {
            var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

            if (chatParameters is not null)
            {
                var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
                var html = studyDay.ToHTML();

                var nextRequest = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: html,
                    parseMode: ParseMode.Html,
                    replyMarkup: dateTime.ToNavigationKeyboard()
                );

                _stepRequestStorage.PushRequest
                (
                    nextRequest,
                    HandleIncomingDateAsync
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

        private async Task SendScheduleAsync(long chatId, DateTime dateTime, string groupTitle)
        {
            var group = await _scheduleParser.ParseGroupAsync(groupTitle);

            if (group is not null)
            {
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
                var html = studyDay.ToHTML();

                var nextRequest = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: html,
                    parseMode: ParseMode.Html,
                    replyMarkup: dateTime.ToNavigationKeyboard()
                );

                _stepRequestStorage.PushRequest
                (
                    nextRequest,
                    HandleIncomingDateAsync,
                    groupTitle
                );
            }
            else
            {
                await _client.SendTextMessageAsync
                (
                    chatId,
                    text: $"Группа под названием \"{groupTitle}\" не найдена. Пожалуйста, уточните запрос",
                    parseMode: ParseMode.Html
                );
            }
        }
    }
}
