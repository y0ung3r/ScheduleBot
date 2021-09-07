using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public delegate Task StepResponseDelegate(Message request, Update response, params object[] payload);
}
