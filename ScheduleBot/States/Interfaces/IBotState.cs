using System.Threading.Tasks;

namespace ScheduleBot.States.Interfaces
{
    public interface IBotState
    {
        Task HandleAsync(object context);
    }
}
