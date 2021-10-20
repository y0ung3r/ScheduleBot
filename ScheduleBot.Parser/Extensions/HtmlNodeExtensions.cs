using HtmlAgilityPack;
using ScheduleBot.Parser.Models;
using System;
using System.Linq;

namespace ScheduleBot.Parser.Extensions
{
    public static class HtmlNodeExtensions
    {
        public static Faculty ToFaculty(this HtmlNode node)
        {
            var onClickParameters = node.Attributes["onclick"]
                                        .Value
                                        .GetTextBetweenBrackets()
                                        .Split(",");

            var id = Convert.ToInt32
            (
                onClickParameters.First()
            );

            return new Faculty
            {
                Id = id,
                Title = node.InnerText
            };
        }

        public static Group ToGroup(this HtmlNode node)
        {
            var onClickParameters = node.Attributes["onclick"]
                                        .Value
                                        .GetTextBetweenBrackets()
                                        .Split(",");

            var typeId = Convert.ToInt32
            (
                onClickParameters.ElementAt(index: 1)
            );

            var id = Convert.ToInt32
            (
                onClickParameters.ElementAt(index: 2)
            );

            return new Group
            {
                Id = id,
                TypeId = typeId,
                Title = node.InnerText
            };
        }

        public static Letter ToLetter(this HtmlNode node)
        {
            var onClickParameter = node.Attributes["onclick"]
                                       .Value
                                       .GetTextBetweenBrackets();

            var index = Convert.ToInt32(onClickParameter);
            var symbol = char.Parse(node.InnerText);

            return new Letter
            {
                Index = index,
                Symbol = symbol
            };
        }

        public static Teacher ToTeacher(this HtmlNode node)
        {
            var onClickParameters = node.Attributes["onclick"]
                                        .Value
                                        .GetTextBetweenBrackets()
                                        .Split(",");

            var typeId = Convert.ToInt32
            (
                onClickParameters.ElementAt(index: 1)
            );

            var id = Convert.ToInt32
            (
                onClickParameters.ElementAt(index: 2)
            );

            return new Teacher
            {
                Id = id,
                Shortname = node.InnerText,
                TypeId = typeId
            };
        }

        public static Lesson ToLesson(this HtmlNode node)
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
        }
    }
}
