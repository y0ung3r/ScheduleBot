using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Commands
{
    [CommandText("/schedule")]
    public class ScheduleCommand : TelegramCommandBase
    {
        // Примечание!
        // ScheduleCommand собрана на быструю руку и ее нужно переделать...
        // Нужно сделать:
        // 1. Перенести всю бизнес-логику в домен (например, избавиться от static коллекций с неделями)
        // 2. Произвести рефакторинг

        private enum WeekToChoose : int
        {
            Previous = 0,
            Current = 1,
            Next = 2
        }

        private static ICollection<DateTime> _previousWeekDates;
        private static ICollection<DateTime> _currentWeekDates;
        private static ICollection<DateTime> _nextWeekDates;

        static ScheduleCommand()
        {
            var dateTime = DateTime.Today;

            _previousWeekDates = new ReadOnlyCollection<DateTime>
            (
                dateTime.AddDays(value: -7)
                        .GetWeekDates(DayOfWeek.Monday)
                        .SkipLast(count: 1)
                        .ToList()
            );

            _currentWeekDates = new ReadOnlyCollection<DateTime>
            (
                dateTime.GetWeekDates(DayOfWeek.Monday)
                        .SkipLast(count: 1)
                        .ToList()
            );

            _nextWeekDates = new ReadOnlyCollection<DateTime>
            (
                dateTime.AddDays(value: 7)
                        .GetWeekDates(DayOfWeek.Monday)
                        .SkipLast(count: 1)
                        .ToList()
            );
        }

        private readonly ILogger<ScheduleCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        private readonly ILongPollingService _longPollingService;

        private string DateFormat => "dd.MM.yy";

        public ScheduleCommand(ILogger<ScheduleCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
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

            var inlineKeyboard = new InlineKeyboardMarkup
            (
                new List<List<InlineKeyboardButton>>()
                {
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            $"{_previousWeekDates.First().ToString(DateFormat)} - {_previousWeekDates.Last().ToString(DateFormat)}",
                            $"{WeekToChoose.Previous:d}"
                        )
                    },
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            $"Текущая неделя",
                            $"{WeekToChoose.Current:d}"
                        )
                    },
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            $"{_nextWeekDates.First().ToString(DateFormat)} - {_nextWeekDates.Last().ToString(DateFormat)}",
                            $"{WeekToChoose.Next:d}"
                        )
                    }
                }
            );

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            var lastMessage = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите учебную неделю:",
                replyMarkup: inlineKeyboard
            );

            _longPollingService.RegisterStepHandler
            (
                chatId,
                HandleIncomingWeekAsync,
                lastMessage
            );

            _logger?.LogInformation("Schedule command executed");
        }

        private async Task HandleIncomingWeekAsync(Update update, object payload)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var previousMessage = (Message)payload;
            var week = Enum.Parse<WeekToChoose>(callbackQuery.Data);

            var weekDates = week switch
            {
                WeekToChoose.Previous => _previousWeekDates,
                WeekToChoose.Current => _currentWeekDates,
                WeekToChoose.Next => _nextWeekDates,
                _ => _currentWeekDates,
            };

            var inlineKeyboard = weekDates.ToInlineKeyboard
            (
                dateTime => dateTime.ToString(DateFormat),
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
                messageId: previousMessage.MessageId
            );

            var lastMessage = await _client.SendTextMessageAsync
            (
                chatId,
                text: "Выберите дату:",
                replyMarkup: inlineKeyboard
            );

            _longPollingService.RegisterStepHandler
            (
                chatId,
                HandleIncomingDateAsync,
                lastMessage
            );

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async Task HandleIncomingDateAsync(Update update, object payload)
        {
            var callbackQuery = update.CallbackQuery;
            var chatId = callbackQuery.Message.Chat.Id;
            var previousMessage = (Message)payload;
            var dateTime = DateTime.Parse(callbackQuery.Data);

            var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            await _client.DeleteMessageAsync
            (
                chatId,
                messageId: previousMessage.MessageId
            );

            if (chatParameters is not null)
            {
                var stringBuilder = new StringBuilder();

                var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
                var html = studyDay.ToHTML();

                var keyboardButtons = new List<InlineKeyboardButton>();
                var previousDay = dateTime.AddDays(-1);
                var nextDay = dateTime.AddDays(1);

                if (!dateTime.DayOfWeek.Equals(DayOfWeek.Monday))
                {
                    keyboardButtons.Add
                    (
                        InlineKeyboardButton.WithCallbackData
                        (
                            "❮",
                            previousDay.ToString(DateFormat)
                        )
                    );
                }

                if (!dateTime.DayOfWeek.Equals(DayOfWeek.Saturday))
                {
                    keyboardButtons.Add
                    (
                        InlineKeyboardButton.WithCallbackData
                        (
                            "❯",
                            nextDay.ToString(DateFormat)
                        )
                    );
                }

                var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

                var lastMessage = await _client.SendTextMessageAsync
                (
                    chatId,
                    text: html,
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboard
                );

                _longPollingService.RegisterStepHandler
                (
                    chatId,
                    HandleIncomingDateAsync,
                    lastMessage
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

            _logger?.LogInformation("Schedule command processed");

            await _client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }
    }
}
