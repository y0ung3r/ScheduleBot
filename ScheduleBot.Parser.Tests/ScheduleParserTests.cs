using NUnit.Framework;
using ScheduleBot.Parser.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Tests
{
    public class ScheduleParserTests
    {
        private IScheduleParser _scheduleParser;

        [OneTimeSetUp]
        public void Setup()
        {
            _scheduleParser = new ScheduleParser();
        }

        [Test]
        [TestCase("Колледж")]
        [TestCase("Исторический факультет")]
        [TestCase("Факультет математики и информационных технологий")]
        public async Task FacultiesParsingTestAsync(string facultyTitle)
        {
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
            var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);

            Assert.AreEqual(faculty.Title, facultyTitle);
        }

        [Test]
        [TestCase(7, "ПМИ21")]
        [TestCase(8, "ХИМ11")]
        [TestCase(9, "НДО31")]
        public async Task GroupsParsingTestAsync(int facultyId, string groupTitle)
        {
            var groups = await _scheduleParser.ParseGroupsAsync(facultyId);
            var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

            Assert.IsNotNull(group);
        }

        [Test]
        [TestCase(8, 19, 2, "ХИМ11")]
        [TestCase(7, 13, 2, "ПМИ21")]
        [TestCase(10, 9227, 2, "ЭБ21")]
        public async Task GroupParsingTestAsync(int facultyId, int groupId, int groupTypeId, string groupTitle)
        {
            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);

            Assert.AreEqual(group.Title, groupTitle);
        }

        [Test]
        [TestCase(8, 19, 2, "2021-6-2", "Строение вещества", "2021-6-7", "День самостоятельной работы", "2021-6-5")]
        [TestCase(8, 19, 2, "2021-5-31", "Строение вещества", "2021-6-5", "История России", "2021-6-4")]
        [TestCase(8, 19, 2, "2021-6-1", "Аналитическая химия", "2021-6-3", "Строение вещества", "2021-6-2")]
        public async Task StudyDaysParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            string startDate, string startLessonTitle, string endDate, 
            string penultimateLessonTitle, string penultimateDate)
        {
            var startDateTime = DateTime.Parse(startDate);
            var endDateTime = DateTime.Parse(endDate);
            var penultimateDateTime = DateTime.Parse(penultimateDate);
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
        [TestCase(8, 19, 2, "2021-6-2", "Строение вещества", "Пр")]
        [TestCase(8, 19, 2, "2021-5-31", "Строение вещества", "Пр")]
        [TestCase(8, 19, 2, "2021-6-1", "Аналитическая химия", "Лаб")]
        [TestCase(7, 13, 2, "2021-6-22", "Иностранный язык", "Экзамен")]
        public async Task StudyDayParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            string date, string lessonTitle, string lessonType)
        {
            var dateTime = DateTime.Parse(date);
            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);
            var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
            var studyDayLesson = studyDay.Lessons.FirstOrDefault(lesson => lesson.Title.Equals(lessonTitle));

            Assert.IsNotNull(studyDayLesson);
            Assert.AreEqual(studyDayLesson.Title, lessonTitle);
            Assert.AreEqual(studyDayLesson.Type, lessonType);
        }
    }
}