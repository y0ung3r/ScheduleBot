using System;
using System.Collections.Generic;

namespace ScheduleBot.Commands.Interfaces
{
    public interface ICommandManager
    {
        ICollection<Type> GetCommandTypesInAssembly();

        Type GetCommandType(string commandPattern);

        void ExecuteCommand(string commandPattern);
    }
}
