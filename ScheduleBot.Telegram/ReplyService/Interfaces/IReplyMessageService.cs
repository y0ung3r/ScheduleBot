using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.ReplyService.Interfaces
{
    public interface IReplyMessageService
    {
        void RegisterRequest(Message message, ReplyCallback callback, params object[] payload);

        void UnregisterRequest(long chatId);

        RequestInfo GetRequestInfo(long chatId);
    }
}
