using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.StepHandler.Interfaces
{
    public interface ICallbackQueryListener
    {
        void RegisterRequest(Message message, CallbackQueryResponseDelegate callback, params object[] payload);

        void UnregisterRequest(long chatId);

        CallbackQueryRequestInfo GetRequestInfo(long chatId, int requestMessageId);

        void ClearRequests();
    }
}
