using ScheduleBot.Commands.Attributes;
using ScheduleBot.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScheduleBot.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly IServiceProvider _serviceProvider;
        private object _executedCommand;

        public CommandManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICollection<Type> GetReservedCommands()
        {
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where
                           (
                               type => type.IsDefined
                               (
                                   typeof(CommandAttribute)
                               )
                           )
                           .ToList();
        }

        public void StartCommand<TCommand>()
        {
            var commandType = typeof(TCommand);

            _executedCommand = Activator.CreateInstance
            (
                commandType
            );

            var methods = commandType.GetMethods();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters()
                                       .Select
                                       (
                                           parameterInfo => _serviceProvider.GetService(parameterInfo.ParameterType)
                                       )
                                       .ToArray();

                method.Invoke(_executedCommand, parameters);
            }
        }
    }
}
