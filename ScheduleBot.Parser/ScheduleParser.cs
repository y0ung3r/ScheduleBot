using Flurl.Http;
using ScheduleBot.Parser.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = ScheduleBot.Parser.Models.Group;

namespace ScheduleBot.Parser
{
    public class ScheduleParser : IScheduleParser
    {
        public async Task<ICollection<Faculty>> ParseFacultiesAsync()
        {
            var page = await ParserRoutes.BaseUrl
                                         .GetStringAsync()
                                         .ToHtmlDocumentAsync();

            return page.DocumentNode
                       .SelectNodes(".//*[contains(@class, 'menu')]/*[contains(@class, 'red')]")
                       .Select
                       (
                           node => node.ToFaculty()
                       )
                       .ToList();
        }

        public async Task<Faculty> ParseFacultyAsync(int facultyId)
        {
            var faculties = await ParseFacultiesAsync();

            return faculties.FirstOrDefault(faculty => faculty.Id.Equals(facultyId));
        }

        public async Task<ICollection<Group>> ParseGroupsAsync(int facultyId)
        {
            var page = await ParserRoutes.GetGroupsUrl(facultyId)
                                         .GetStringAsync()
                                         .ToHtmlDocumentAsync();

            return page.DocumentNode
                       .SelectNodes(".//ul/li/a")
                       .Select
                       (
                           node => node.ToGroup()
                       )
                       .ToList();
        }

        public async Task<Group> ParseGroupAsync(int facultyId, int groupId, int groupTypeId)
        {
            var groups = await ParseGroupsAsync(facultyId);

            return groups.FirstOrDefault(group => group.Id.Equals(groupId) && group.TypeId.Equals(groupTypeId));
        }

        public async Task<Group> ParseGroupAsync(string groupTitle)
        {
            var faculties = await ParseFacultiesAsync();

            foreach (var faculty in faculties)
            {
                var groups = await ParseGroupsAsync(faculty.Id);
                var groupTitleInUpperCase = groupTitle.ToUpper();

                var group = groups.FirstOrDefault
                (
                    group => group.Title
                                  .ToUpper()
                                  .Equals(groupTitleInUpperCase)
                );

                if (group is not null)
                {
                    return group;
                }
            }

            return default;
        }

        public async Task<ICollection<Letter>> ParseLettersAsync()
        {
            var page = await ParserRoutes.LettersUrl
                                         .GetStringAsync()
                                         .ToHtmlDocumentAsync();

            return page.DocumentNode
                       .SelectNodes(".//*[contains(@id, 'letter')]")
                       .Select
                       (
                           node => node.ToLetter()
                       )
                       .ToList();
        }

        public async Task<ICollection<Teacher>> ParseTeachersAsync(string filterText = default)
        {
            var letters = await ParseLettersAsync();
            var teachers = new List<Teacher>();

            foreach (var letter in letters)
            {
                var page = await ParserRoutes.GetTeachersUrl(letter.Index)
                                             .GetStringAsync()
                                             .ToHtmlDocumentAsync();

                var nodes = page.DocumentNode
                                .SelectNodes(".//*[contains(@class, 'prep_list_col')]/*[contains(@class, 'prep_name')]")
                                .Where(node =>
                                {
                                    var onClickParameters = node.Attributes["onclick"]
                                                                .Value
                                                                .GetTextBetweenBrackets();

                                    return !Regex.IsMatch(onClickParameters, ",{2,}");
                                });

                if (!string.IsNullOrWhiteSpace(filterText))
                {
                    var filterTextInUpperCase = filterText.ToUpper();

                    nodes.Where
                    (
                        node => node.InnerText
                                    .ToUpper()
                                    .Contains(filterTextInUpperCase)
                    );
                }

                teachers.AddRange
                (
                    nodes.Select
                    (
                        node => node.ToTeacher()
                    )
                    .ToList()
                );
            }

            return teachers;
        }

        public async Task<ICollection<StudyDay>> ParseStudyDaysAsync(Group group, DateTime startDateTime, DateTime endDateTime)
        {
            if (startDateTime > endDateTime)
            {
                throw new ArgumentException($"\"{nameof(startDateTime)}\" can not be earlier than \"{nameof(endDateTime)}\"");
            }

            var studyDays = new List<StudyDay>();

            for (var dateTime = startDateTime; dateTime <= endDateTime; dateTime = dateTime.AddDays(value: 1))
            {
                if (!dateTime.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    studyDays.Add
                    (
                        await ParseStudyDayAsync(group, dateTime)
                    );
                }
            }

            return studyDays;
        }

        public async Task<StudyDay> ParseStudyDayAsync(Group group, DateTime dateTime)
        {
            if (dateTime.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                throw new ArgumentException($"The \"{nameof(dateTime)}\" cannot be Sunday");
            }

            var studyDay = new StudyDay()
            {
                Date = dateTime.Date,
                WeekNumber = dateTime.GetRelativeWeek(DateTime.Today, DayOfWeek.Monday)
            };

            var content = new FormUrlEncodedContent
            (
                new Dictionary<string, string>()
                {
                    {
                        "type",
                        $"{group.TypeId}"
                    },
                    {
                        "id",
                        $"{group.Id}"
                    },
                    {
                        "week",
                        $"{studyDay.WeekNumber}"
                    }
                }
            );

            var page = await ParserRoutes.LessonsUrl
                                         .PostAsync(content)
                                         .ReceiveString()
                                         .ToHtmlDocumentAsync();

            var dayNode = page.DocumentNode
                              .SelectNodes(".//*[contains(@class, 'day')]")
                              .FirstOrDefault(node =>
                              {
                                  var dateFromSchedule = DateTime.Parse
                                  (
                                      node.SelectNodes(".//*[contains(@class, 'date student')]")
                                          .FirstOrDefault()
                                          .InnerText
                                          .Split(" ")
                                          .Last()
                                  );

                                  return dateFromSchedule.Date.CompareTo(dateTime.Date).Equals(0);
                              });

            if (dayNode != null)
            {
                var lessons = dayNode.SelectNodes(".//ul/*[contains(@class, 'lesson add_background')]")?
                                     .Select
                                     (
                                         node => node.ToLesson()
                                     )
                                     .ToList();

                if (lessons != null)
                {
                    studyDay.Lessons = lessons;
                }
            }

            return studyDay;
        }
    }
}
