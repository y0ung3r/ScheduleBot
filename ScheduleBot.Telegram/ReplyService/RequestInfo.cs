using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.ReplyService
{
    public class RequestInfo
    {
        public Message Message { get; set; }

        public ReplyCallback Callback { get; set; }

        public object[] Payload { get; set; }
    }
}
