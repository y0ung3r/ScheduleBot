using System.Text;
using BotFramework.Context.Interfaces;
using BotFramework.Handlers.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands;

public class StartCommand : IUpdateHandler<Message, ITelegramBotClient>, IWithAsyncPrerequisite<Message>
{
	public async Task HandleAsync(Message command, IBotContext<ITelegramBotClient> context)
	{
		var chatId = command.Chat.Id;
		var stringBuilder = new StringBuilder();

		stringBuilder.AppendLine("Чтобы обеспечить себе быстрый доступ к расписанию, используйте команду /bind.")
					 .AppendLine("Также Вы можете добавить бота в свою Telegram-группу.")
					 .AppendLine()
					 .AppendLine("<i>Бот не хранит Ваши личные данные!</i>")
					 .AppendLine()
					 .AppendLine("<b>Вы можете использовать следующие команды:</b>")
					 .AppendLine()
					 .AppendLine("1. /settings - получить текущие настройки факультета и группы")
					 .AppendLine()
					 .AppendLine("2. /bind - изменить настройки факультета и группы")
					 .AppendLine()
					 .AppendLine("3. /schedule - получить актуальное расписание по заданной учебной неделе")
					 .AppendLine()
					 .AppendLine("4. /schedule <i>[дата (в любом формате)]</i> - получить актуальное расписание по заданному учебному дню")
					 .AppendLine("Пример: <code>/schedule 01.09.2021</code>")
					 .AppendLine()
					 .AppendLine("5. /schedule <i>[дата (в любом формате)]</i> <i>[название группы]</i> - получить актуальное расписание по заданному учебному дню")
					 .AppendLine("Пример: <code>/schedule 2021-9-1 ПМИ31</code>")
					 .AppendLine()
					 .AppendLine("6. /tomorrow - получить расписание на завтра")
					 .AppendLine()
					 .AppendLine("7. /tomorrow <i>[название группы]</i> - получить расписание на завтра для указанной группы.")
					 .AppendLine("Пример: <code>/tomorrow ХИМ21</code>");


		await context.Client.SendChatActionAsync
		(
			chatId,
			chatAction: ChatAction.Typing
		);

		await context.Client.SendTextMessageAsync
		(
			chatId,
			text: stringBuilder.ToString(),
			parseMode: ParseMode.Html,
			disableWebPagePreview: true
		);
	}

	public Task<bool> CanHandleAsync(Message command)
	{
		return Task.FromResult(command.Text == "/start");
	}
}