using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<IList<TSource>> ChunkBy<TSource>(this ICollection<TSource> source, int columnsCount)
        {
            return source.Select((element, index) => new
            {
                Index = index,
                Value = element
            })
            .GroupBy(element => element.Index / columnsCount)
            .Select
            (
                element => element.Select(x => x.Value).ToList() as IList<TSource>
            )
            .ToList();
        }

        public static IReplyMarkup ToReplyKeyboard<TSource, TValue>(this ICollection<TSource> source, 
            Func<TSource, TValue> keySelector, int columnsCount, 
            bool resizeKeyboard = false, bool oneTimeKeyboard = false)
        {
            var keyboardButtons = source.Select(element => new KeyboardButton()
            {
                Text = keySelector(element).ToString()
            })
            .ToList();

            return new ReplyKeyboardMarkup()
            {
                Keyboard = keyboardButtons.ChunkBy(columnsCount),
                ResizeKeyboard = resizeKeyboard,
                OneTimeKeyboard = oneTimeKeyboard
            };
        }

        public static IReplyMarkup ToInlineKeyboard<TSource, TValue>(this ICollection<TSource> source, 
            Func<TSource, TValue> keySelector, int columnsCount)
        {
            var keyboardButtons = source.Select
            (
                element => InlineKeyboardButton.WithCallbackData
                (
                    keySelector(element).ToString()
                )
            )
            .ToList();

            return new InlineKeyboardMarkup
            (
                keyboardButtons.ChunkBy(columnsCount)
            );
        }
    }
}
