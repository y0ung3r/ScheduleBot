using BotFramework.Context.Interfaces;
using BotFramework.Handlers.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands;

public class BindCommand : IUpdateHandler<Message, ITelegramBotClient>, IWithAsyncPrerequisite<Message>
{
	private readonly IScheduleParser _scheduleParser;
	
	public BindCommand(IScheduleParser scheduleParser)
	{
		_scheduleParser = scheduleParser;
	}
	
	public async Task HandleAsync(Message command, IBotContext<ITelegramBotClient> context)
	{
		var chatId = command.Chat.Id;
		
		var faculties = await _scheduleParser.ParseFacultiesAsync();
		var facultiesKeyboard = faculties.ToInlineKeyboard
		(
			faculty => faculty.TitleAbbreviation,
			columnsCount: 2
		);

		await context.Client.SendChatActionAsync
		(
			chatId,
			chatAction: ChatAction.Typing
		);

		var facultyRequest = await context.Client.SendTextMessageAsync
		(
			chatId,
			text: "Выберите факультет, к которому Вас нужно прикрепить:",
			replyMarkup: facultiesKeyboard
		);

		var facultyResponse = await context.WaitNextUpdateAsync<CallbackQuery>();
		
		await context.Client.DeleteMessageAsync
		(
			chatId,
			messageId: facultyRequest.MessageId
		);
		
		var facultyAbbreviation = facultyResponse.Data;
		var faculty = faculties.FirstOrDefault(faculty => faculty.TitleAbbreviation.Equals(facultyAbbreviation));
		
		if (faculty is not null)
		{
			var groups = await _scheduleParser.ParseGroupsAsync(faculty.Id);
			var groupsKeyboard = groups.ToInlineKeyboard
			(
				group => group.Title,
				columnsCount: 3
			);

			await context.Client.SendChatActionAsync
			(
				chatId,
				chatAction: ChatAction.Typing
			);

			var groupRequest = await context.Client.SendTextMessageAsync
			(
				chatId,
				text: "Теперь выберите группу:",
				replyMarkup: groupsKeyboard
			);
			
			var groupResponse = await context.WaitNextUpdateAsync<CallbackQuery>();
			
			await context.Client.DeleteMessageAsync
			(
				chatId,
				messageId: groupRequest.MessageId
			);
			
			var groupTitle = groupResponse.Data;
			var group = groups.FirstOrDefault(group => group.Title.Equals(groupTitle));

			if (group is not null)
			{
				await context.Client.SendChatActionAsync
				(
					chatId,
					chatAction: ChatAction.Typing
				);

				/*await _chatParametersService.SaveChatParametersAsync
				(
					chatId,
					facultyId,
					group.Id,
					group.TypeId
				);*/

				await context.Client.SendTextMessageAsync
				(
					chatId,
					text: "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
				);
			}

			await context.Client.AnswerCallbackQueryAsync(groupResponse.Id);
		}

		await context.Client.AnswerCallbackQueryAsync(facultyResponse.Id);
	}

	public Task<bool> CanHandleAsync(Message command)
	{
		return Task.FromResult(command.Text == "/bind");
	}
}