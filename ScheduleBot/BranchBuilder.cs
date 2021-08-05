using Microsoft.Extensions.DependencyInjection;
using ScheduleBot.Handlers;
using ScheduleBot.Handlers.Interfaces;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;

namespace ScheduleBot
{
    public class BranchBuilder : IBranchBuilder
    {
        private readonly Stack<Func<RequestDelegate, RequestDelegate>> _branch;

        public IServiceProvider ServiceProvider { get; }

        public BranchBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            _branch = new Stack<Func<RequestDelegate, RequestDelegate>>();
        }

        private RequestDelegate BuildRootHandler()
        {
            var rootHandler = default(RequestDelegate);

            foreach (var handler in _branch)
            {
                rootHandler = handler(rootHandler);
            }

            return rootHandler;
        }

        public IBranchBuilder UseHandler(IRequestHandler handler)
        {
            _branch.Push
            (
                next => request => handler.HandleAsync(request, next)
            );

            return this;
        }

        public IBranchBuilder UseHandler<TRequestHandler>()
            where TRequestHandler : IRequestHandler
        {
            return UseHandler
            (
                ServiceProvider.GetRequiredService<TRequestHandler>()
            );
        }

        public IBranchBuilder UseInternalHandler(Predicate<object> predicate, Action<IBranchBuilder> configure)
        {
            var branchBuilder = ServiceProvider.GetRequiredService<IBranchBuilder>();
            configure(branchBuilder);

            var branch = branchBuilder.Build();
            var internalHandlerFactory = ServiceProvider.GetRequiredService<Func<RequestDelegate, Predicate<object>, InternalHandler>>();

            return UseHandler
            (
                internalHandlerFactory
                (
                    branch,
                    predicate
                )
            );
        }

        public RequestDelegate Build()
        {
            UseHandler<MissingRequestHandler>();

            return BuildRootHandler();
        }
    }
}
