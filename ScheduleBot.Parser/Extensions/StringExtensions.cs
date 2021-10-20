using System.Linq;
using System.Text.RegularExpressions;

namespace ScheduleBot.Parser.Extensions
{
    public static class StringExtensions
    {
        public static string GetTextBetweenBrackets(this string text)
        {
            return Regex.Match(text, @"\(([\s\S]+?)\)")
                        .Groups
                        .Values
                        .LastOrDefault()
                        .Value;
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

                            return splitBySpaces.Any(splitWord => splitWord.Equals(word) && splitWord.Equals("и")) ? symbol : upperSymbol;
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
