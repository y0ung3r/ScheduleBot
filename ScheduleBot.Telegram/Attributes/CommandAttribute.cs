using System;

namespace ScheduleBot.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Pattern { get; }

        public CommandAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}
