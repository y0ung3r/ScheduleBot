using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Systems
{
    public class NotificationSystem : SystemBase
    {
        public override async Task OnMessageReceivedAsync(Message message)
        {
            await Bot.Client.SendTextMessageAsync
            (
                message.Chat.Id,
                "Сообщение получено системой уведомлений!"
            );
        }
    }
}
