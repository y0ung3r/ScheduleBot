using ScheduleBot.Commands.Attributes;
using ScheduleBot.Commands.Interfaces;
using System.Linq;

namespace ScheduleBot.Extensions
{
    public static class CommandExtensions
    {
        public static bool ContainsRoute(this IBotCommand command, string route)
        {
            return command.GetType()
                          .GetCustomAttributes
                          (
                              typeof(RouteAttribute),
                              inherit: false
                          )
                          .Select(attribute =>
                          {
                              return attribute as RouteAttribute;
                          })
                          .Any(routeAttribute =>
                          {
                              return routeAttribute.Route.Equals(route);
                          });
        }
    }
}
