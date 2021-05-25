using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Systems
{
    [Command(pattern: "/schedule")]
    public class FetchScheduleSystem : TelegramSystemBase
    {
        public enum WeekToChoose : int
        {
            Previous = 0,
            Current = 1,
            Next = 2
        }

        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;
        private ICollection<DateTime> _previousWeekDates;
        private ICollection<DateTime> _currentWeekDates;
        private ICollection<DateTime> _nextWeekDates;
        private string DateFormat => "dd.MM.yy";

        public FetchScheduleSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
        }

        private async Task AskWeekAsync(long chatId)
        {
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

            await Client.SendTextMessageAsync
            (
                chatId,
                "Выберите учебную неделю:",
                replyMarkup: inlineKeyboard
            );
        }

        private async Task AskDateAsync(long chatId, WeekToChoose week)
        {
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

            await Client.SendTextMessageAsync
            (
                chatId,
                "Выберите дату:",
                replyMarkup: inlineKeyboard
            );
        }

        private async Task SendScheduleAsync(long chatId, DateTime dateTime)
        {
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParameters(chatId);

            await Client.SendChatActionAsync(chatId, ChatAction.Typing);

            if (chatParameters is not null)
            {
                var stringBuilder = new StringBuilder();

                var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);

                stringBuilder.AppendLine($"<b>Расписание на {dateTime.ToShortDateString()}:</b>")
                             .AppendLine();

                if (studyDay.Lessons.Count > 0)
                {
                    foreach (var lesson in studyDay.Lessons)
                    {
                        stringBuilder.AppendLine($"<b>{lesson.Number} {lesson.Title}</b>");

                        if (!string.IsNullOrWhiteSpace(lesson.Type))
                        {
                            stringBuilder.Append($"<i>{lesson.Type}</i>");

                            if (!string.IsNullOrWhiteSpace(lesson.ClassroomNumber))
                            {
                                stringBuilder.Append($" <i>{lesson.ClassroomNumber}</i>");
                            }

                            stringBuilder.Append($" в {lesson.TimeRange}");
                        }

                        stringBuilder.AppendLine();

                        if (lesson.Teachers.Count > 0)
                        {
                            stringBuilder.AppendJoin(", ", lesson.Teachers);
                        }

                        stringBuilder.AppendLine()
                                     .AppendLine();
                    }
                }
                else
                {
                    stringBuilder.AppendLine("Пары отсутствуют");
                }

                var keyboardButtons = new List<InlineKeyboardButton>();
                var previousDay = dateTime.AddDays(-1);
                var nextDay = dateTime.AddDays(1);

                if (!previousDay.DayOfWeek.Equals(DayOfWeek.Monday))
                {
                    keyboardButtons.Add
                    (
                        InlineKeyboardButton.WithCallbackData
                        (
                            "🢀",
                            $"{previousDay.ToString(DateFormat)}"
                        )
                    );
                }

                if (!nextDay.DayOfWeek.Equals(DayOfWeek.Saturday))
                {
                    keyboardButtons.Add
                    (
                        InlineKeyboardButton.WithCallbackData
                        (
                            "🢂",
                            $"{nextDay.ToString(DateFormat)}"
                        )
                    );
                }

                var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

                await Client.SendTextMessageAsync
                (
                    chatId,
                    stringBuilder.ToString(),
                    ParseMode.Html,
                    replyMarkup: inlineKeyboard
                );
            }
            else
            {
                await Client.SendTextMessageAsync
                (
                    chatId,
                    "Вы не настроили бота, чтобы использовать этот функционал. Используйте /bind, чтобы начать работу",
                    ParseMode.Html
                );
            }
        }

        public override Task OnInitializeAsync()
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

            return Task.CompletedTask;
        }

        public override async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = callbackQuery.Data;

            if (Enum.TryParse(callbackData, out WeekToChoose week))
            {
                await AskDateAsync(chatId, week);
            }
            else if (DateTime.TryParse(callbackData, out DateTime dateTime))
            {
                await SendScheduleAsync(chatId, dateTime);
            }

            await Client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        protected override async Task OnCommandReceivedAsync(Message command)
        {
            var chatId = command.Chat.Id;

            await Client.SendChatActionAsync(chatId, ChatAction.Typing);
            await AskWeekAsync(chatId);
        }
    }
}