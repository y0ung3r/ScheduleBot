using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot.Parser.Extensions
{
    public static class StringExtensions
    {
        public static ICollection<string> GetTextBetweenBrackets(this string text)
        {
            var textBetweenBrackets = string.Concat
            (
                text.SkipWhile(symbol => !symbol.Equals('('))
                    .Skip(count: 1)
                    .TakeWhile(symbol => !symbol.Equals(')'))
            );

            return textBetweenBrackets.Split(',');
        }
    }
}
