using ScheduleBot.Commands.Attributes;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Extensions;
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

        internal void InvokeCommandInitializer(object command, params object[] parameters)
        {
            var initializerMethod = command.GetType()
                                           .GetMethods()
                                           .FirstOrDefault
                                           (
                                               method => method.GetCustomAttribute<CommandInitializerAttribute>() != null
                                           );

            if (initializerMethod != null)
            {
                var services = initializerMethod.GetParameters()
                                                .Select
                                                (
                                                    parameterInfo => _serviceProvider.GetService(parameterInfo.ParameterType)
                                                )
                                                .ToArray();

                initializerMethod.Invoke(command, parameters);
            }
        }

        public ICollection<Type> GetCommandTypesInAssembly()
        {
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where
                           (
                               type => type.IsCommand()
                           )
                           .ToList();
        }

        public Type GetCommandType(string commandPattern)
        {
            return GetCommandTypesInAssembly().FirstOrDefault(type => type.MessageIsCommand(commandPattern));
        }

        public void ExecuteCommand(string commandPattern)
        {
            var commandType = GetCommandType(commandPattern);
            _executedCommand = _serviceProvider.GetService(commandType) ?? Activator.CreateInstance(commandType);

            InvokeCommandInitializer(_executedCommand);
        }
    }
}
