using System;

namespace ScheduleBot.Telegram.LongPolling
{
    public class StepHandlerInfo
    {
        public StepDelegate Callback { get; set; }

        public object[] Payload { get; set; }
    }
}
