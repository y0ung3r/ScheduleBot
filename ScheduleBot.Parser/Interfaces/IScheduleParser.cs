﻿using ScheduleBot.Parser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Interfaces
{
    public interface IScheduleParser
    {
        Task<ICollection<Faculty>> ParseFacultiesAsync();

        Task<Faculty> ParseFacultyAsync(int facultyId);

        Task<ICollection<Group>> ParseGroupsAsync(int facultyId);

        Task<Group> ParseGroupAsync(int facultyId, int groupId, int groupTypeId);

        Task<Group> ParseGroupAsync(string groupTitle);

        Task<ICollection<StudyDay>> ParseStudyDaysAsync(Group group, DateTime startDateTime, DateTime endDateTime);

        Task<StudyDay> ParseStudyDayAsync(Group group, DateTime dateTime);
    }
}
