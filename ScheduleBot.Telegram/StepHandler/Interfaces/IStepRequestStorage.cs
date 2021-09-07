using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler.Interfaces
{
    public interface IStepRequestStorage
    {
        void PushRequest(Message message, StepResponseDelegate callback, params object[] payload);

        void RemoveChatRequests(long chatId);

        StepRequestInfo GetRequestInfo(long chatId, int requestMessageId);
    }
}
