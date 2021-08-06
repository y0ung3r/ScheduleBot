using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.LongPolling
{
    public delegate Task StepDelegate(Update request, params object[] payload);
}
