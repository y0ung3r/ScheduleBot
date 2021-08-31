using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public delegate Task CallbackQueryResponseDelegate(Message request, CallbackQuery response, params object[] payload);
}
