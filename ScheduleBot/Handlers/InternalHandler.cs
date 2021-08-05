using Microsoft.Extensions.Logging;
using ScheduleBot.Handlers.Interfaces;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Handlers
{
    internal class InternalHandler : IRequestHandler
    {
        private readonly ILogger<InternalHandler> _logger;
        private readonly RequestDelegate _branchRootHandler;
        private readonly Predicate<object> _predicate;

        public InternalHandler(ILogger<InternalHandler> logger, RequestDelegate branch, Predicate<object> predicate)
        {
            _logger = logger;
            _branchRootHandler = branch;
            _predicate = predicate;
        }

        public Task HandleAsync(object request, RequestDelegate nextHandler)
        {
            if (_predicate(request))
            {
                _logger?.LogInformation("A request will be redirected to the branch");

                return _branchRootHandler(request);
            }

            _logger?.LogInformation("A request will be redirected to the next handler");

            return nextHandler(request);
        }
    }
}