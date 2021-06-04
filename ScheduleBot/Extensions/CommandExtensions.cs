using ScheduleBot.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class CommandExtensions
    {
        internal static CommandAttribute GetCommandAttribute(this Type commandType)
        {
            return commandType.GetCustomAttributes(inherit: false)
                              .AsEnumerable()
                              .FirstOrDefault(attribute => attribute is CommandAttribute) as CommandAttribute;
        }

        public static IEnumerable<string> GetCommandPatterns(this Type commandType)
        {
            return commandType.GetCommandAttribute()
                              .Pattern
                              .Split(",");
        }

        public static bool IsCommand(this Type commandType)
        {
            return commandType.GetCommandAttribute() != null;
        }

        public static bool MessageIsCommand(this Type commandType, string message)
        {
            if (commandType.IsCommand())
            {
                return false;
            }

            return commandType.GetCommandPatterns()
                              .Any
                              (
                                  pattern => pattern.Trim()
                                                    .Equals(message)
                              );
        }
    }
}
