using ScheduleBot.Attributes;
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
            await client.SendTextMessageAsync
            (
                command.Chat.Id,
                "Вы можете использовать следующие команды:" +
                "\n\n" +
                "<b>Настройки:</b>\n" +
                "/settings - посмотреть текущие настройки, необходимые для получения расписания\n" +
                "/setup - изменить настройки, необходимые для получения расписания" +
                "\n\n" +
                "<b>Возможности:</b>\n" +
                "/schedule - получить расписание на сегодня",
                ParseMode.Html,
                disableWebPagePreview: true
            );
        }
    }
}
