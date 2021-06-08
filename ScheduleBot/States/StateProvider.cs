using ScheduleBot.States.Interfaces;
using System;

namespace ScheduleBot.States
{
    public class StateProvider : IStateProvider
    {
        public IBotState CreateState<TState>()
            where TState : IBotState
        {
            return Activator.CreateInstance<TState>();
        }
    }
}
