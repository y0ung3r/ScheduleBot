namespace ScheduleBot.Data.Models
{
    public class UserSchedule
    {
        public int Id { get; set; }

        public long ChatId { get; set; }

        public int FacultyId { get; set; }

        public int GroupId { get; set; }

        public int GroupTypeId { get; set; }
    }
}
