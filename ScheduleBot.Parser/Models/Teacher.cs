namespace ScheduleBot.Parser.Models
{
    /// <summary>
    /// Преподаватель
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// Идентификатор преподавателя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Фамилия и инициалы преподавателя
        /// </summary>
        public string Shortname { get; set; }

        /// <summary>
        /// Идентификатор типа преподавателя
        /// </summary>
        public int TypeId { get; set; }
    }
}
