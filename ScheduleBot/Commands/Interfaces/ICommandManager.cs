using System;
using System.Collections.Generic;

namespace ScheduleBot.Commands.Interfaces
{
    public interface ICommandManager
    {
        ICollection<Type> GetReservedCommands();

        void StartCommand<TCommand>();
    }
}
