namespace ScheduleBot.Domain.DTO
{
    public class ChatParametersDTO
    {
        public long ChatId { get; set; }

        public int FacultyId { get; set; }

        public int GroupId { get; set; }

        public int GroupTypeId { get; set; }

        public string FacultyTitleWithoutTag { get; set; }

        public string GroupTitle { get; set; }
    }
}
