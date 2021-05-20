using ScheduleBot.Parser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Interfaces
{
    public interface IScheduleParser
    {
        Task<ICollection<Faculty>> ParseFacultiesAsync();

        Task<ICollection<Group>> ParseGroupsAsync(int facultyId);

        Task<Group> ParseGroupAsync(int facultyId, int groupId, int groupTypeId);

        Task<ICollection<Lesson>> ParseLessonsAsync(Group group, DateTime dateTime);
    }
}
