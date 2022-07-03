using System.Text;
using BotFramework.Context.Interfaces;
using BotFramework.Handlers.Interfaces;
using ScheduleBot.Parser.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands;

public class SettingsCommand : IUpdateHandler<Message, ITelegramBotClient>, IWithAsyncPrerequisite<Message>
{
	private readonly IScheduleParser _scheduleParser;
	
	public SettingsCommand(IScheduleParser scheduleParser)
	{
		_scheduleParser = scheduleParser;
	}
	
	public async Task HandleAsync(Message command, IBotContext<ITelegramBotClient> context)
	{
		var chatId = command.Chat.Id;
		//var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<b>Текущие настройки:</b>");

		/*if (chatParameters is not null)
		{
			stringBuilder.AppendLine($"Факультет: <i>{chatParameters.FacultyTitleWithoutTag}</i>")
						 .AppendLine($"Группа: <i>{chatParameters.GroupTitle}</i>");
		}
		else*/
		{
			stringBuilder.AppendLine("<i>Отсутствуют</i>")
						 .AppendLine("Используйте /bind, чтобы начать работу с ботом");
		}

		await context.Client.SendChatActionAsync
		(
			chatId,
			chatAction: ChatAction.Typing
		);

		await context.Client.SendTextMessageAsync
		(
			chatId,
			text: stringBuilder.ToString(),
			parseMode: ParseMode.Html
		);

	}

	public Task<bool> CanHandleAsync(Message command)
	{
		return Task.FromResult(command.Text == "/settings");
	}
}