using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly ICollection<IBotCommand> _commands;

        public CommandManager()
        {
            _commands = new List<IBotCommand>();
        }

        public void RegisterCommand(IBotCommand command)
        {
            _commands.Add(command);
        }

        public void UnregisterCommand(IBotCommand command)
        {
            _commands.Remove(command);
        }

        public IBotCommand FindCommand(string route)
        {
            return _commands.FirstOrDefault
            (
                command => command.ContainsRoute(route)
            );
        }

        public bool TryFindCommand(string route, out IBotCommand command)
        {
            command = FindCommand(route);

            return command != null;
        }
    }
}
