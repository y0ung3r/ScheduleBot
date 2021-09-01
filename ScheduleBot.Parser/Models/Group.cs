namespace ScheduleBot.Parser.Models
{
    /// <summary>
    /// Группа
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Идентификатор группы
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование группы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор типа группы
        /// </summary>
        public int TypeId { get; set; }
    }
}
