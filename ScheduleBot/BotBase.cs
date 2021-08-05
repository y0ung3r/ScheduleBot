﻿using ScheduleBot.Interfaces;

namespace ScheduleBot
{
    public abstract class BotBase : IBot
    {
        protected readonly RequestDelegate _rootHandler;

        public BotBase(RequestDelegate rootHandler)
        {
            _rootHandler = rootHandler;
        }

        public abstract void Run();

        public abstract void Stop();
    }
}
