﻿using ScheduleBot.Handlers.Interfaces;
using System;
using System.Threading.Tasks;

namespace ScheduleBot.Tests
{
    public class FakeCommandHandler : ICommandHandler
    {
        public bool CanHandle(IServiceProvider serviceProvider, object request) => true;

        public Task HandleAsync(object request, RequestDelegate nextHandler) => Task.CompletedTask;
    }
}
