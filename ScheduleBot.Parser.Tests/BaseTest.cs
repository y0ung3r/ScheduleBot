using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using ScheduleBot.Parser.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Parser.Tests
{
    public abstract class BaseTest
    {
        private readonly AutoMocker _mocker;

        protected string _html;
        protected IScheduleParser _scheduleParser;

        public BaseTest()
        {
            _mocker = new AutoMocker();
        }

        [SetUp]
        public void Setup()
        {
            _mocker.Setup<IRestClient, Task<string>>
            (
                restClient => restClient.GetAsync
                (
                    It.IsAny<Uri>()
                )
            )
            .ReturnsAsync
            (
                () => _html
            );

            _mocker.Setup<IRestClient, Task<string>>
            (
                restClient => restClient.PostAsync
                (
                    It.IsAny<Uri>(),
                    It.IsAny<HttpContent>()
                )
            )
            .ReturnsAsync
            (
                () => _html
            );

            _scheduleParser = _mocker.CreateInstance<ScheduleParser>();
        }

        protected void LoadHtmlFromFile(string filepath)
        {
            var path = $"{Path.Combine(AppContext.BaseDirectory, "Data", filepath)}.html";

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            _html = File.ReadAllText(path, Encoding.UTF8);
        }

        [TearDown]
        public void Cleanup()
        {
            _html = null;
        }
    }
}
