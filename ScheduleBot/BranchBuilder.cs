using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Handlers;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot
{
    public class BranchBuilder : IBranchBuilder
    {
        private readonly Stack<IRequestHandler> _handlers;

        public IServiceProvider ServiceProvider { get; }

        public BranchBuilder(IServiceProvider serviceProvider)
        {
            _handlers = new Stack<IRequestHandler>();

            ServiceProvider = serviceProvider;
        }

        public IBranchBuilder UseHandler(IRequestHandler handler)
        {
            _handlers.Push(handler);

            return this;
        }

        public IBranchBuilder UseBranch(Predicate<object> predicate, Action<IBranchBuilder> configure)
        {
            var branchBuilder = ServiceProvider.GetRequiredService<IBranchBuilder>();
            configure(branchBuilder);

            var branch = branchBuilder.Build();
            var internalHandlerFactory = ServiceProvider.GetRequiredService<Func<RequestDelegate, Predicate<object>, InternalHandler>>();

            return UseHandler
            (
                internalHandlerFactory(branch, predicate)
            );
        }

        public RequestDelegate Build() 
        {
            var rootHandler = default(RequestDelegate);

            var branch = _handlers.Select
            (
                handler => new Func<RequestDelegate, RequestDelegate>
                (
                    next => request => handler.HandleAsync(request, next)
                )
            );

            foreach (var handler in branch)
            {
                rootHandler = handler(rootHandler);
            }

            return rootHandler;
        }
    }
}
