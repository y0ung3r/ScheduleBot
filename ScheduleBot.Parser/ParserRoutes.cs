using Flurl;

namespace ScheduleBot.Parser
{
    public static class ParserRoutes
    {
        public static Url BaseUrl => "http://edu.strbsu.ru";

        public static Url ListUrl => $"{BaseUrl}/php/getList.php";

        public static Url LettersUrl => $"{ListUrl}?prepList=1";

        public static Url LessonsUrl => $"{BaseUrl}/php/getShedule.php";

        public static Url GetGroupsUrl(int facultyId) => $"{ListUrl}?faculty={facultyId}";

        public static Url GetTeachersUrl(int letterIndex) => $"{ListUrl}?letter={letterIndex}";
    }
}