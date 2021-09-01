using System.Collections.Generic;

namespace ScheduleBot.Parser.Models
{
    /// <summary>
    /// Занятие (пара)
    /// </summary>
    public class Lesson 
    {
        /// <summary>
        /// Номер занятия (н-р: 1-ая пара)
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Наименование занятия
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Время начала и конца занятия
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// Тип (лекция, практика и т. д.)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Номер аудитории
        /// </summary>
        public string ClassroomNumber { get; set; }

        /// <summary>
        /// Список преподавателей, которые проводят занятие
        /// </summary>
        public ICollection<string> Teachers { get; set; }
    }
}
