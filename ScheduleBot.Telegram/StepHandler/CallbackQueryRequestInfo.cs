using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public class CallbackQueryRequestInfo
    {
        public Message Message { get; set; }

        public CallbackQueryResponseDelegate Callback { get; set; }

        public object[] Payload { get; set; }
    }
}
