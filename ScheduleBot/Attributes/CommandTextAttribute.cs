using System;

namespace ScheduleBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandTextAttribute : Attribute
    {
        public string CommandText { get; }

        public CommandTextAttribute(string commandText)
        {
            CommandText = commandText;
        }
    }
}
