using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class StringExtensions
    {
        public static string GetTextBetweenBrackets(this string text)
        {
            var textBetweenBrackets = string.Concat
            (
                text.SkipWhile(symbol => !symbol.Equals('('))
                    .Skip(count: 1)
                    .TakeWhile(symbol => !symbol.Equals(')'))
            );

            return textBetweenBrackets;
        }

        public static string AsInSentences(this string text)
        {
            var trimmedText = text.Trim().ToLower();
            return char.ToUpper(trimmedText[0]) + trimmedText.Substring(startIndex: 1, trimmedText.Length - 1);
        }
    }
}
