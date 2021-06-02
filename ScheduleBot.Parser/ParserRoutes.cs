using System;

namespace ScheduleBot.Parser
{
    public static class ParserRoutes
    {
        public static string BaseUrl => "http://edu.strbsu.ru";

        public static Uri GetBaseUri() => new Uri(BaseUrl);

        public static Uri GetGroupsUri(int facultyId) => new Uri($"{BaseUrl}/php/getList.php?faculty={facultyId}");

        public static Uri GetLessonsUri() => new Uri($"{BaseUrl}/php/getShedule.php");
    }
}