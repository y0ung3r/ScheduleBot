using System.Threading.Tasks;

namespace ScheduleBot.Commands.Interfaces
{
    public interface IBotCommand
    {
        Task ExecuteAsync();
    }
}
