using ScheduleBot.Telegram.LongPolling.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleBot.Telegram.LongPolling
{
    public class StepHandlerStorage : IStepHandlerStorage
    {
        private readonly IDictionary<long, StepHandlerInfo> _stepHandlersInfo;

        public StepHandlerStorage()
        {
            _stepHandlersInfo = new Dictionary<long, StepHandlerInfo>();
        }

        public bool IsStepHandlerRegistered(long chatId)
        {
            return _stepHandlersInfo.ContainsKey(chatId);
        }

        public void RegisterStepHandler(long chatId, StepDelegate callback, object payload = null)
        {
            if (!IsStepHandlerRegistered(chatId))
            {
                _stepHandlersInfo.Add
                (
                    chatId, 
                    new StepHandlerInfo
                    {
                        Callback = callback,
                        Payload = payload
                    }
                );
            }
        }

        public StepHandlerInfo GetStepHandlerInfo(long chatId)
        {
            var stepHandlerInfo = default(StepHandlerInfo);

            if (IsStepHandlerRegistered(chatId))
            {
                stepHandlerInfo = _stepHandlersInfo[chatId];
            }

            return stepHandlerInfo;
        }

        public void ClearChatStepHandler(long chatId)
        {
            if (IsStepHandlerRegistered(chatId))
            {
                _stepHandlersInfo.Remove(chatId);
            }
        }

        public void ClearStepHandlers()
        {
            _stepHandlersInfo.Clear();
        }
    }
}
