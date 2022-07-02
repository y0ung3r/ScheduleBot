using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram;

public class WebhookConfiguration : IHostedService
{
    private readonly BotOptions _botOptions;
    private readonly ITelegramBotClient _client;

    public WebhookConfiguration(IOptions<BotOptions> botOptions, ITelegramBotClient client)
    {
        _botOptions = botOptions.Value;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = @$"{_botOptions.WebHookAddress}/Bot/GetUpdates";
            
        await _client.SetWebhookAsync
        (
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}