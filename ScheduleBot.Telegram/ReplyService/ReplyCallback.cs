using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.ReplyService
{
    public delegate Task ReplyCallback(Message request, Update response, params object[] payload);
}
