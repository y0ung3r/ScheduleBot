﻿using ScheduleBot.Parser.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScheduleBot.Parser
{
    public class ScheduleParser : IScheduleParser
    {
        private readonly HttpClient _httpClient;

        public ScheduleParser()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ICollection<Faculty>> ParseFacultiesAsync()
        {
            var page = await _httpClient.CreateHtmlDocumentAsync
            (
                ParserRoutes.GetBaseUri()
            );

            return page.DocumentNode
                       .SelectNodes(".//*[contains(@class, 'menu')]/*[contains(@class, 'red')]")
                       .Select(node =>
                       {
                           var onClickParameters = node.Attributes["onclick"]
                                                       .Value
                                                       .GetTextBetweenBrackets()
                                                       .Split(",");

                           var id = Convert.ToInt32
                           (
                               onClickParameters.First()
                           );

                           return new Faculty()
                           {
                               Id = id,
                               Title = node.InnerText
                           };
                       })
                       .ToList();
        }

        public async Task<Faculty> ParseFacultyAsync(int facultyId)
        {
            var faculties = await ParseFacultiesAsync();

            return faculties.FirstOrDefault(faculty => faculty.Id.Equals(facultyId));
        }

        public async Task<ICollection<Group>> ParseGroupsAsync(int facultyId)
        {
            var page = await _httpClient.CreateHtmlDocumentAsync
            (
                ParserRoutes.GetGroupsUri(facultyId)
            );

            return page.DocumentNode
                       .SelectNodes(".//ul/li/a")
                       .Select(node =>
                       {
                           var onClickParameters = node.Attributes["onclick"]
                                                       .Value
                                                       .GetTextBetweenBrackets()
                                                       .Split(",");

                           var type = Convert.ToInt32
                           (
                               onClickParameters.ElementAt(index: 1)
                           );

                           var id = Convert.ToInt32
                           (
                               onClickParameters.ElementAt(index: 2)
                           );

                           return new Group()
                           {
                               Id = id,
                               TypeId = type,
                               Title = node.InnerText
                           };
                       })
                       .ToList();
        }

        public async Task<Group> ParseGroupAsync(int facultyId, int groupId, int groupTypeId)
        {
            var groups = await ParseGroupsAsync(facultyId);

            return groups.FirstOrDefault(group => group.Id.Equals(groupId) && group.TypeId.Equals(groupTypeId));
        }

        public async Task<ICollection<StudyDay>> ParseStudyDaysAsync(Group group, DateTime startDateTime, DateTime endDateTime)
        {
            if (startDateTime > endDateTime)
            {
                throw new ArgumentException($"\"{nameof(startDateTime)}\" can be earlier than \"{nameof(endDateTime)}\"");
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
                throw new ArgumentException($"The \"{nameof(dateTime)}\" cannot be sunday");
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

            var page = await _httpClient.CreateHtmlDocumentAsync
            (
                ParserRoutes.GetLessonsUri(),
                content
            );

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
                                     .Select(node =>
                                     {
                                         var number = node.SelectNodes(".//*[contains(@class, 'number')]")
                                                          .Nodes()
                                                          .FirstOrDefault()
                                                          .InnerText;

                                         var title = node.SelectNodes(".//*[contains(@class, 'name')]")
                                                         .Nodes()
                                                         .FirstOrDefault()
                                                         .InnerText;

                                         var timeRange = node.SelectNodes(".//*[contains(@class, 'time')]")
                                                             .Nodes()
                                                             .FirstOrDefault()
                                                             .InnerText;

                                         var type = node.SelectNodes(".//*[contains(@class, 'type')]")
                                                        .Nodes()
                                                        .LastOrDefault()
                                                        .InnerText
                                                        .Trim();

                                         var classroomNumber = node.SelectNodes(".//*[contains(@class, 'cab')]")
                                                                   .Nodes()
                                                                   .FirstOrDefault()
                                                                   .InnerText;

                                         var teachers = node.SelectNodes(".//*[contains(@class, 'prep')]")
                                                            .Nodes()
                                                            .SelectMany(node => node.ChildNodes)
                                                            .Select
                                                            (
                                                                node => node.InnerText.Replace("(", " (")
                                                            )
                                                            .ToList();

                                         return new Lesson
                                         {
                                             Number = number,
                                             Title = title,
                                             TimeRange = timeRange,
                                             Type = type,
                                             ClassroomNumber = classroomNumber,
                                             Teachers = teachers
                                         };
                                     })
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
