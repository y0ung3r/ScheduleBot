using System.Linq;

namespace ScheduleBot.Parser.Models
{
    public class Faculty
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Abbreviation
        {
            get
            {
                var abbreviation = default(string);
                var splitBySpaces = Title.Split(" ");

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
                    abbreviation = Title;
                }

                return abbreviation;
            }
        }
    }
}
