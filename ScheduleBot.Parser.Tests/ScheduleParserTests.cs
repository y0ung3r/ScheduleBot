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
        [TestCase("�������")]
        [TestCase("������������ ���������")]
        [TestCase("��������� ���������� � �������������� ����������")]
        public async Task FacultiesParsingTestAsync(string facultyTitle)
        {
            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var faculty = faculties.FirstOrDefault(faculty => faculty.Title.Contains(facultyTitle));

            Assert.IsNotNull(faculty);
        }

        [Test]
        [TestCase(4, "�������������� ���������")]
        [TestCase(5, "��������� ���������� � �������� ���������")]
        [TestCase(6, "������������ ���������")]
        public async Task FacultyParsingTestAsync(int facultyId, string facultyTitle)
        {
            var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);

            Assert.AreEqual(faculty.Title, facultyTitle);
        }

        [Test]
        [TestCase(7, "���21")]
        [TestCase(8, "���11")]
        [TestCase(9, "���31")]
        public async Task GroupsParsingTestAsync(int facultyId, string groupTitle)
        {
            var groups = await _scheduleParser.ParseGroupsAsync(facultyId);
            var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

            Assert.IsNotNull(group);
        }

        [Test]
        [TestCase(8, 19, 2, "���11")]
        [TestCase(7, 13, 2, "���21")]
        [TestCase(10, 9227, 2, "��21")]
        public async Task GroupParsingTestAsync(int facultyId, int groupId, int groupTypeId, string groupTitle)
        {
            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);

            Assert.AreEqual(group.Title, groupTitle);
        }

        [Test]
        [TestCase(8, 19, 2, "2021-6-2", "�������� ��������", "2021-6-7", "���� ��������������� ������", "2021-6-5")]
        [TestCase(8, 19, 2, "2021-5-31", "�������� ��������", "2021-6-5", "������� ������", "2021-6-4")]
        [TestCase(8, 19, 2, "2021-6-1", "������������� �����", "2021-6-3", "�������� ��������", "2021-6-2")]
        public async Task StudyDaysParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            DateTime startDateTime, string startLessonTitle, DateTime endDateTime, 
            string penultimateLessonTitle, DateTime penultimateDateTime)
        {
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
        [TestCase(8, 19, 2, "2021-6-2", "�������� ��������", "��")]
        [TestCase(8, 19, 2, "2021-5-31", "�������� ��������", "��")]
        [TestCase(8, 19, 2, "2021-6-1", "������������� �����", "���")]
        [TestCase(7, 13, 2, "2021-6-22", "����������� ����", "�������")]
        public async Task StudyDayParsingTestAsync(int facultyId, int groupId, int groupTypeId, 
            DateTime dateTime, string lessonTitle, string lessonType)
        {
            var group = await _scheduleParser.ParseGroupAsync(facultyId, groupId, groupTypeId);
            var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);
            var studyDayLesson = studyDay.Lessons.FirstOrDefault(lesson => lesson.Title.Equals(lessonTitle));

            Assert.IsNotNull(studyDayLesson);
            Assert.AreEqual(studyDayLesson.Title, lessonTitle);
            Assert.AreEqual(studyDayLesson.Type, lessonType);
        }
    }
}