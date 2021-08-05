using System.Linq;

namespace ScheduleBot.Parser.Extensions
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
            var trimmedText = text.Trim()
                                  .ToLower();

            return char.ToUpper(trimmedText[0]) + trimmedText.Substring(startIndex: 1, trimmedText.Length - 1);
        }

        public static string RemoveSubstring(this string text, string substring)
        {
            var loweredText = text.ToUpper();
            var loweredSubstring = substring.ToUpper();

            return text.Remove
            (
                loweredText.IndexOf(loweredSubstring),
                count: loweredSubstring.Length
            )
            .Trim()
            .AsInSentences();
        }

        public static string ToAbbreviation(this string text)
        {
            var abbreviation = default(string);
            var splitBySpaces = text.Split(" ");

            if (splitBySpaces.Length > 1)
            {
                abbreviation = string.Concat
                (
                    splitBySpaces.Select
                    (
                        word =>
                        {
                            var symbol = word.First();

                            var upperSymbol = char.ToUpper
                            (
                                symbol
                            );

                            var isAnd = splitBySpaces.FirstOrDefault(splitWord => splitWord.Equals(word) && splitWord.Equals("и")) != null;

                            return (isAnd) ? symbol : upperSymbol;
                        }
                    )
                );
            }
            else
            {
                abbreviation = text;
            }

            return abbreviation;
        }
    }
}
