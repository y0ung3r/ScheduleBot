using ScheduleBot.Attributes;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/start, /help")]
    public class MainSystem : SystemBase
    {
        public override async Task OnCommandReceivedAsync(ITelegramBotClient client, Message command)
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

            await client.SendTextMessageAsync
            (
                command.Chat.Id,
                stringBuilder.ToString(),
                ParseMode.Html,
                disableWebPagePreview: true
            );
        }
    }
}
