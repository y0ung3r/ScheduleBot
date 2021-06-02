using System;
using System.Threading.Tasks;

namespace ScheduleBot.Interfaces
{
    public interface IBot
    {
        Task OnUpdateReceivedAsync(object update);

        Task OnErrorReceivedAsync(Exception exception);
    }
}
