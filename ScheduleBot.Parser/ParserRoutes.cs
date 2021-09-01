using System;

namespace ScheduleBot.Parser
{
    public static class ParserRoutes
    {
        public static string BaseUrl => "http://edu.strbsu.ru";

        public static string ListUrl => $"{BaseUrl}/php/getList.php";

        public static Uri GetBaseUri() => new Uri(BaseUrl);

        public static Uri GetGroupsUri(int facultyId) => new Uri($"{ListUrl}?faculty={facultyId}");

        public static Uri GetLettersUri() => new Uri($"{ListUrl}?prepList=1");

        public static Uri GetTeachersUri(int letterIndex) => new Uri($"{ListUrl}?letter={letterIndex}");

        public static Uri GetLessonsUri() => new Uri($"{BaseUrl}/php/getShedule.php");
    }
}