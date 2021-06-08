using ScheduleBot.Commands.Interfaces;
using ScheduleBot.States.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.States
{
    public class ResponseState : IBotState
    {
        public async Task HandleAsync(object context)
        {
            if (context is IBotCommand command)
            {
                await command.ExecuteAsync();
            }
        }
    }
}
