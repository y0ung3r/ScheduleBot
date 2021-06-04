using System;

namespace ScheduleBot.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandInitializerAttribute : Attribute
    { }
}
