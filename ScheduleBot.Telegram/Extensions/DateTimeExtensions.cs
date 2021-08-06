using ScheduleBot.Domain.Extensions;
using ScheduleBot.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Extensions
{
    public static class DateTimeExtensions
    {
        public static IReplyMarkup ToWeekDatesKeyboard(this DateTime dateTime, IWeekDatesProvider weekDatesProvider)
        {
            var previousWeekDates = weekDatesProvider.GetPreviousWeekDates(dateTime);

            var previousWeekStartDate = previousWeekDates.First()
                                                         .ToShortDateString();

            var previousWeekEndDate = previousWeekDates.Last()
                                                       .ToShortDateString();

            var nextWeekDates = weekDatesProvider.GetNextWeekDates(dateTime);

            var nextWeekStartDate = nextWeekDates.First()
                                                 .ToShortDateString();

            var nextWeekEndDate = nextWeekDates.Last()
                                               .ToShortDateString();

            return new InlineKeyboardMarkup
            (
                new List<List<InlineKeyboardButton>>()
                {
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            text: $"{previousWeekStartDate} - {previousWeekEndDate}",
                            callbackData: previousWeekStartDate
                        )
                    },
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            text: $"Текущая неделя",
                            callbackData: dateTime.ToShortDateString()
                        )
                    },
                    new List<InlineKeyboardButton>()
                    {
                        InlineKeyboardButton.WithCallbackData
                        (
                            text: $"{nextWeekStartDate} - {nextWeekEndDate}",
                            callbackData: nextWeekStartDate
                        )
                    }
                }
            );
        }

        public static IReplyMarkup ToNavigationKeyboard(this DateTime dateTime)
        {
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
                        previousDay.ToShortDateString()
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
                        nextDay.ToShortDateString()
                    )
                );
            }

            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}
