using ScheduleBot.Attributes;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Systems
{
    [Command(pattern: "/start, /help")]
    public class MainSystem : TelegramSystemBase
    {
        protected override async Task OnCommandReceivedAsync(Message command)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Вы можете использовать команды из следующих категорий:")
                         .AppendLine()
                         .AppendLine("<b>Настройки:</b>")
                         .AppendLine("/settings - посмотреть текущие настройки, необходимые для получения расписания")
                         .AppendLine("/bind - изменить настройки, необходимые для получения расписания")
                         .AppendLine()
                         .AppendLine("<b>Возможности:</b>")
                         .AppendLine("/schedule - получить расписание на сегодня");

            await Client.SendTextMessageAsync
            (
                command.Chat.Id,
                stringBuilder.ToString(),
                ParseMode.Html,
                disableWebPagePreview: true
            );
        }
    }
}
