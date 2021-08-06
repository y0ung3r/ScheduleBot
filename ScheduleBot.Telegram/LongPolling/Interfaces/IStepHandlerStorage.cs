namespace ScheduleBot.Telegram.LongPolling.Interfaces
{
    public interface IStepHandlerStorage
    {
        bool IsStepHandlerRegistered(long chatId);

        void RegisterStepHandler(long chatId, StepDelegate callback, params object[] payload);

        StepHandlerInfo GetStepHandlerInfo(long chatId);

        void ClearChatStepHandler(long chatId);

        void ClearStepHandlers();
    }
}
