using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public class StepRequestInfo
    {
        public Message Message { get; set; }

        public StepResponseDelegate Callback { get; set; }

        public object[] Payload { get; set; }
    }
}
