namespace ScheduleBot.Parser.Models
{
    /// <summary>
    /// Буква, являющаяся первой в фамилии преподавателя
    /// </summary>
    public class Letter
    {
        /// <summary>
        /// Индекс буквы
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Символ
        /// </summary>
        public char Symbol { get; set; }
    }
}
