using System.Text;
using System.Threading.Tasks;
using BotFramework;
using BotFramework.Attributes;
using Microsoft.Extensions.Logging;
using ScheduleBot.Domain.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands.Settings
{
	[CommandAliases("/settings")]
	public class SettingsCommand : TelegramCommandBase
	{
		private readonly ILogger<SettingsCommand> _logger;
		private readonly IChatParametersService _chatParametersService;

		public SettingsCommand(ILogger<SettingsCommand> logger, ITelegramBotClient client, IChatParametersService chatParametersService)
			: base(client)
		{
			_logger = logger;
			_chatParametersService = chatParametersService;
		}

		public override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
		{
			var chatId = message.Chat.Id;
			var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<b>Текущие настройки:</b>");

			if (chatParameters is not null)
			{
				stringBuilder.AppendLine($"Факультет: <i>{chatParameters.FacultyTitleWithoutTag}</i>")
							 .AppendLine($"Группа: <i>{chatParameters.GroupTitle}</i>");
			}
			else
			{
				stringBuilder.AppendLine("Отсутствуют")
					         .AppendLine("Используйте /bind, чтобы начать работу с ботом");
			}

			await Client.SendChatActionAsync(chatId, ChatAction.Typing);

			await Client.SendTextMessageAsync
			(
				chatId,
				text: stringBuilder.ToString(),
				parseMode: ParseMode.Html
			);

			_logger?.LogInformation("Settings command processed");
		}
	}
}