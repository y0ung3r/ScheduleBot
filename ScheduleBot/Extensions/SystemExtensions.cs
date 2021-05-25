using ScheduleBot.Attributes;
using ScheduleBot.Interfaces;
using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class SystemExtensions
    {
        private static CommandAttribute GetCommandAttribute(this ISystem system)
        {
            return system.GetType()
                         .GetCustomAttributes(inherit: false)
                         .ToList()
                         .FirstOrDefault(attribute => attribute is CommandAttribute) as CommandAttribute;
        }

        public static bool IsCommand(this ISystem system)
        {
            return system.GetCommandAttribute() != null;
        }

        public static bool MessageIsCommand(this ISystem system, string message)
        {
            var pattern = system.GetCommandAttribute()
                                .Pattern
                                .Split(",")
                                .FirstOrDefault
                                (
                                    pattern => pattern.Trim()
                                                      .Equals(message)
                                );

            return system.IsCommand() && pattern != null;
        }
    }
}
