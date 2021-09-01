using ScheduleBot.Parser.Extensions;

namespace ScheduleBot.Parser.Models
{
    /// <summary>
    /// Факультет
    /// </summary>
    public class Faculty
    {
        /// <summary>
        /// Идентификатор факультета
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование факультета
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Наименование без слова "Факультет" в начале
        /// </summary>
        public string TitleWithoutFacultyTag => Title.RemoveSubstring("Факультет");

        /// <summary>
        /// Аббревиатура
        /// </summary>
        public string TitleAbbreviation => Title.ToAbbreviation();
    }
}
