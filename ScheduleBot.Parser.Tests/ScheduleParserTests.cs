using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Tests
{
    public class ScheduleParserTests : BaseTest
    {
        [Test]
        [TestCase("Колледж")]
        [TestCase("Исторический факультет")]
        [TestCase("Факультет математики и информационных технологий")]
        public async Task FacultiesParsingTestAsync(string facultyTitle)
        {
            LoadHtmlFromFile("FacultiesPage");

            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var faculty = faculties.FirstOrDefault(faculty => faculty.Title.Contains(facultyTitle));

            Assert.IsNotNull(faculty);
        }

        [Test]
        [TestCase(4, "Филологический факультет")]
        [TestCase(5, "Факультет башкирской и тюркской филологии")]
        [TestCase(6, "Исторический факультет")]
        public async Task FacultyParsingTestAsync(int facultyId, string facultyTitle)
        {
            LoadHtmlFromFile("FacultiesPage");

            var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);

            Assert.AreEqual(faculty.Title, facultyTitle);
        }

        [Test]
        [TestCase(7, "ПМИ21", "GroupsPage (ФМиИТ)")]
        [TestCase(8, "ХИМ11", "GroupsPage (ЕНФ)")]
        public async Task GroupsParsingTestAsync(int facultyId, string groupTitle, string filepath)
        {
            LoadHtmlFromFile(filepath);

            var groups = await _scheduleParser.ParseGroupsAsync(facultyId);
            var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

            Assert.IsNotNull(group);
        }

        [Test]
        [TestCase(7, 13, 2, "ПМИ21")]
        [TestCase(7, 10288, 2, "ММИ21")]
        [TestCase(7, 16, 2, "АИС21")]
        public async Task GroupParsingTestAsync(int facultyId, int groupId, int groupTypeId, string groupTitle)
        {
            LoadHtmlFromFile("GroupsPage (ФМиИТ)");

            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);

            Assert.AreEqual(group.Title, groupTitle);
        }

        [Test]
        [TestCase("ПМИ21")]
        [TestCase("ПМИ31")]
        [TestCase("ПМИ41")]
        [TestCase("МИ11")]
        [TestCase("аис21")]
        public async Task GroupParsingByTitleTestAsync(string groupTitle)
        {
            LoadHtmlFromFile("GroupsPage");

            var group = await _scheduleParser.ParseGroupAsync(groupTitle);

            Assert.AreEqual
            (
                group.Title.ToUpper(), 
                groupTitle.ToUpper()
            );
        }

        [Test]
        [TestCase(7, 13, 2, "2021-9-1", "Архитектура компьютеров", "2021-9-4", "Дискретная математика", "2021-9-3")]
        [TestCase(7, 13, 2, "2021-9-1", "Архитектура компьютеров", "2021-9-2", "Архитектура компьютеров", "2021-9-1")]
        public async Task StudyDaysParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            DateTime startDateTime, string startLessonTitle, DateTime endDateTime, 
            string penultimateLessonTitle, DateTime penultimateDateTime)
        {
            LoadHtmlFromFile("StudyDaysPage");

            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);
            var studyDays = await _scheduleParser.ParseStudyDaysAsync(group, startDateTime, endDateTime);
            var startStudyDay = studyDays.FirstOrDefault();
            var startLesson = startStudyDay.Lessons.FirstOrDefault();
            var penultimateStudyDay = studyDays.SkipLast(count: 1).LastOrDefault();
            var penultimateLesson = penultimateStudyDay.Lessons.FirstOrDefault();

            Assert.IsNotNull(startLesson);
            Assert.IsNotNull(penultimateLesson);
            Assert.AreEqual(startStudyDay.Date, startDateTime);
            Assert.AreEqual(penultimateStudyDay.Date, penultimateDateTime);
            Assert.AreEqual(startLesson.Title, startLessonTitle);
            Assert.AreEqual(penultimateLesson.Title, penultimateLessonTitle);
        }

        [Test]
        [TestCase(7, 13, 2, "2021-9-1", "Архитектура компьютеров", "Лек")]
        [TestCase(7, 13, 2, "2021-9-2", "Web-программирование", "Лек")]
        public async Task StudyDayParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            DateTime dateTime, string lessonTitle, string lessonType)
        {
            LoadHtmlFromFile("StudyDaysPage");

            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);
            var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
            var studyDayLesson = studyDay.Lessons.FirstOrDefault(lesson => lesson.Title.Equals(lessonTitle));

            Assert.IsNotNull(studyDayLesson);
            Assert.AreEqual(studyDayLesson.Title, lessonTitle);
            Assert.AreEqual(studyDayLesson.Type, lessonType);
        }
    }
}