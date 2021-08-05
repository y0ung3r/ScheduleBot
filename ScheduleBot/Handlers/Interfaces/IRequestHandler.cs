using System.Threading.Tasks;

namespace ScheduleBot.Handlers.Interfaces
{
    public interface IRequestHandler
    {
        Task HandleAsync(object request, RequestDelegate nextHandler);
    }
}