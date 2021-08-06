using ScheduleBot.Telegram.StepHandler.Interfaces;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler
{
    public class CallbackQueryListener : ICallbackQueryListener
    {
        private readonly IDictionary<long, CallbackQueryRequestInfo> _requests;

        public CallbackQueryListener()
        {
            _requests = new Dictionary<long, CallbackQueryRequestInfo>();
        }

        public void RegisterRequest(Message message, CallbackQueryResponseDelegate callback, params object[] payload)
        {
            var chatId = message.Chat.Id;

            if (!_requests.ContainsKey(chatId))
            {
                _requests.Add
                (
                    chatId,
                    new CallbackQueryRequestInfo
                    {
                        Message = message,
                        Callback = callback,
                        Payload = payload
                    }
                );
            }
            else
            {
                var request = _requests[chatId];
                request.Message = message;
                request.Callback = callback;
                request.Payload = payload;
            }
        }

        public void UnregisterRequest(long chatId)
        {
            if (_requests.ContainsKey(chatId))
            {
                _requests.Remove(chatId);
            }
        }

        public CallbackQueryRequestInfo GetRequestInfo(long chatId, int requestMessageId)
        {
            if (_requests.TryGetValue(chatId, out CallbackQueryRequestInfo requestInfo) && requestInfo.Message.MessageId.Equals(requestMessageId))
            {
                return requestInfo;
            }

            return default(CallbackQueryRequestInfo);
        }

        public void ClearRequests() => _requests.Clear();
    }
}
