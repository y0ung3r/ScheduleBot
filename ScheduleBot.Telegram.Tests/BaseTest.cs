using Moq.AutoMock;
using NUnit.Framework;

namespace ScheduleBot.Telegram.Tests
{
    public class BaseTest
    {
        private readonly AutoMocker _mocker;

        public BaseTest()
        {
            _mocker = new AutoMocker();
        }

        [SetUp]
        public void Setup()
        {

        }
    }
}