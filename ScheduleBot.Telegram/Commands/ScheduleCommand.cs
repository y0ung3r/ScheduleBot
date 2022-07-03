using BotFramework.Context.Interfaces;
using BotFramework.Handlers.Interfaces;
using ScheduleBot.Parser.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Commands;

public class ScheduleCommand : IUpdateHandler<Message, ITelegramBotClient>, IWithAsyncPrerequisite<Message>
{
	private readonly IScheduleParser _scheduleParser;
	
	public ScheduleCommand(IScheduleParser scheduleParser)
	{
		_scheduleParser = scheduleParser;
	}

	private async Task AskGroupIfNotBinded(Message command, IBotContext<ITelegramBotClient> context)
	{
		
	}
	
	public async Task HandleAsync(Message command, IBotContext<ITelegramBotClient> context)
	{
		AskGroupIfNotBinded(command, context);
	}

	public Task<bool> CanHandleAsync(Message command)
	{
		return Task.FromResult(command.Text == "/schedule");
	}
}