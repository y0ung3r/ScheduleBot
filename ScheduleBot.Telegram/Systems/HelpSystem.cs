using ScheduleBot.Attributes;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/start, /help")]
    public class HelpSystem : SystemBase
    {
        public override async Task OnCommandReceivedAsync(Message message)
        {
            await Bot.Client.SendTextMessageAsync
            (
                message.Chat.Id,
                "Вы можете использовать следующие команды:" +
                "\n\n" +
                "<b>Настройки:</b>\n" +
                "/settings - посмотреть текущие настройки, необходимые для получения расписания\n" +
                "/setup - изменить настройки, необходимые для получения расписания" +
                "\n\n" +
                "<b>Возможности:</b>\n" +
                "/schedule - получить расписание на сегодня" +
                "\n\n" +
                "<b>Расписание СФ БашГУ</b> - проект с открытым исходным кодом\n" +
                "<b>GitHub:</b> https://github.com/y0ung3r/ScheduleBot",
                ParseMode.Html,
                disableWebPagePreview: true
            );
        }
    }
}
