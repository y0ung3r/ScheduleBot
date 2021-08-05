using Microsoft.Extensions.Logging;
using ScheduleBot.Handlers.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Handlers
{
    public class MissingRequestHandler : IRequestHandler
    {
        private readonly ILogger<MissingRequestHandler> _logger;

        public MissingRequestHandler(ILogger<MissingRequestHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(object request, RequestDelegate nextHandler)
        {
            _logger.LogWarning($"No handler for request with type: {request.GetType()}");

            return Task.CompletedTask;
        }
    }
}
