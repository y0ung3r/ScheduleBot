using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Extensions
{
    public static class CollectionExtensions
    {
        public static List<List<TSource>> ChunkBy<TSource>(this ICollection<TSource> source, int chunkSize)
        {
            return source.Select((element, index) => new 
                         { 
                             Index = index, 
                             Value = element
                         })
                         .GroupBy(element => element.Index / chunkSize)
                         .Select
                         (
                             element => element.Select(x => x.Value).ToList()
                         )
                         .ToList();
        }

        public static IReplyMarkup ToReplyKeyboard<TSource, TProperty>(this ICollection<TSource> source, Expression<Func<TSource, TProperty>> property, 
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
                Keyboard = keyboardButtons.ChunkBy(chunkSize: 4),
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }
    }
}
