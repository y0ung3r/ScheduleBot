using BotFramework.Extensions;
using ScheduleBot.Parser.Extensions;
using ScheduleBot.Telegram;
using ScheduleBot.Telegram.Commands;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var botSection = builder.Configuration.GetSection("Bot");
var botOptions = botSection.Get<BotOptions>();
services.Configure<BotOptions>(botSection);

services.AddEndpointsApiExplorer()
		.AddSwaggerGen();

services.AddHostedService<WebhookConfiguration>();

services.AddHttpClient("tgwebhook")
		.AddTypedClient<ITelegramBotClient>
		(
			httpClient => new TelegramBotClient
			(
				botOptions.Token, 
				httpClient
			)
		);
     
services.AddScheduleParser()
	    .AddBotFramework<ITelegramBotClient>()
		.AddHandler<StartCommand>()
		.AddHandler<BindCommand>()
	    .AddHandler<SettingsCommand>();
            
services.AddControllers()
		.AddNewtonsoftJson();

var application = builder.Build();
var environment = application.Environment;

if (environment.IsDevelopment())
{
	application.UseSwagger()
			   .UseSwaggerUI();
}

application.UseHttpsRedirection()
		   .UseAuthorization()
		   .UseRouting();

application.MapControllers();

application.Run();