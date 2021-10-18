using ScheduleBot.Handlers.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Tests
{
    public class FakeRequestHandler : IRequestHandler
    {
        public Task HandleAsync(object request, RequestDelegate nextHandler) => Task.CompletedTask;
    }
}
