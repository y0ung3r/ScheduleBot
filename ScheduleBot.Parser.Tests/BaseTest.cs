using Flurl.Http.Testing;
using Moq.AutoMock;
using NUnit.Framework;
using ScheduleBot.Parser.Interfaces;
using System;
using System.IO;
using System.Text;

namespace ScheduleBot.Parser.Tests
{
    public abstract class BaseTest
    {
        public BaseTest()
        {
            _mocker = new AutoMocker();
        }

        [SetUp]
        public void Setup()
        {
            _httpTest = new HttpTest();
            _scheduleParser = _mocker.CreateInstance<ScheduleParser>();
        }

        protected void LoadHtmlFromFile(string filepath)
        {
            var path = $"{Path.Combine(AppContext.BaseDirectory, "Data", filepath)}.html";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var html = File.ReadAllText(path, Encoding.UTF8);
            _httpTest.RespondWith(html);
        }

        [TearDown]
        public void Cleanup()
        {
            _httpTest.Dispose();
        }

        protected IScheduleParser _scheduleParser;
        private HttpTest _httpTest;
        private readonly AutoMocker _mocker;
    }
}
