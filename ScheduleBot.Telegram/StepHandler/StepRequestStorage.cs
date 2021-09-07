using ScheduleBot.Telegram.StepHandler.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public class StepRequestStorage : IStepRequestStorage
    {
        private readonly ICollection<StepRequestInfo> _requestsInfo;

        public StepRequestStorage()
        {
            _requestsInfo = new List<StepRequestInfo>();
        }

        public bool IsRequestContains(long chatId)
        {
            return _requestsInfo.Any
            (
                requestInfo => requestInfo.Message.Chat.Id.Equals(chatId)
            );
        }

        public void PushRequest(Message message, StepResponseDelegate callback, params object[] payload)
        {
            var chatId = message.Chat.Id;

            if (!IsRequestContains(chatId))
            {
                _requestsInfo.Add
                (
                    new StepRequestInfo
                    {
                        Message = message,
                        Callback = callback,
                        Payload = payload
                    }
                );
            }
        }

        public void RemoveChatRequests(long chatId)
        {
            if (IsRequestContains(chatId))
            {
                var chatRequestsInfo = _requestsInfo.Where
                (
                    requestInfo => requestInfo.Message.Chat.Id.Equals(chatId)
                )
                .ToList();

                foreach (var requestInfo in chatRequestsInfo)
                {
                    _requestsInfo.Remove(requestInfo);
                }
            }
        }

        public StepRequestInfo GetRequestInfo(long chatId, int requestMessageId)
        {
            return _requestsInfo.FirstOrDefault
            (
                requestInfo => requestInfo.Message.Chat.Id.Equals(chatId) && 
                               requestInfo.Message.MessageId.Equals(requestMessageId)
            );
        }
    }
}
