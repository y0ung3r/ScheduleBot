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
            return system.IsCommand() && system.GetCommandAttribute().Pattern.Equals(message);
        }
    }
}
