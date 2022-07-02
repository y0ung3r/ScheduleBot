namespace ScheduleBot.Telegram;

/// <summary>
/// Конфигурация для Telegram Bot API
/// </summary>
public class BotOptions
{
    /// <summary>
    /// Токен
    /// </summary>
    public string Token { get; init; }
        
    /// <summary>
    /// Адрес на который осуществляет поставка обновлений от Telegram
    /// </summary>
    public string WebHookAddress { get; init; }
}