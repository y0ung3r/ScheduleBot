using ScheduleBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static IReplyMarkup ToReplyKeyboard<TSource, TProperty>(this ICollection<TSource> source, Expression<Func<TSource, TProperty>> property, int columnsCount, 
            bool resizeKeyboard = false, bool oneTimeKeyboard = false)
        {
            var propertyInfo = property.GetPropertyInfo();
            
            var keyboardButtons = source.Select(element => new KeyboardButton()
            {
                Text = propertyInfo.GetValue(element)
                                   .ToString()
            })
            .ToList();

            return new ReplyKeyboardMarkup()
            {
                Keyboard = keyboardButtons.ChunkBy(columnsCount),
                ResizeKeyboard = resizeKeyboard,
                OneTimeKeyboard = oneTimeKeyboard
            };
        }

        public static IReplyMarkup ToInlineKeyboard<TSource, TProperty>(this ICollection<TSource> source, Expression<Func<TSource, TProperty>> property, int columnsCount)
        {
            var propertyInfo = property.GetPropertyInfo();

            var keyboardButtons = source.Select(element =>
            {
                var text = propertyInfo.GetValue(element)
                                       .ToString();
                                            
                return InlineKeyboardButton.WithCallbackData(text);
            })
            .ToList();

            return new InlineKeyboardMarkup
            (
                keyboardButtons.ChunkBy(columnsCount)
            );
        }
    }
}
