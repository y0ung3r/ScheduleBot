using ScheduleBot.Parser.Extensions;

namespace ScheduleBot.Parser.Models
{
    public class Faculty
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string TitleWithoutFacultyTag => Title.RemoveSubstring("Факультет");

        public string TitleAbbreviation => Title.ToAbbreviation();
    }
}
