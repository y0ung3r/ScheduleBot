using ScheduleBot.Telegram.ReplyService.Interfaces;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.ReplyService
{
    public class ReplyMessageService : IReplyMessageService
    {
        private readonly IDictionary<long, RequestInfo> _requests;

        public ReplyMessageService()
        {
            _requests = new Dictionary<long, RequestInfo>();
        }

        public void RegisterRequest(Message message, ReplyCallback callback, params object[] payload)
        {
            _requests.Add
            (
                message.Chat.Id,
                new RequestInfo
                {
                    Message = message,
                    Callback = callback,
                    Payload = payload
                }
            );
        }

        public void UnregisterRequest(long chatId)
        {
            if (_requests.ContainsKey(chatId))
            {
                _requests.Remove(chatId);
            }
        }

        public RequestInfo GetRequestInfo(long chatId)
        {
            if (_requests.TryGetValue(chatId, out var requestInfo))
            {
                return requestInfo;
            }

            return default;
        }
    }
}
