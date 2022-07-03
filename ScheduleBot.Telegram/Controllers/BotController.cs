using BotFramework.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
	private readonly IUpdateReceiver _receiver;

	public BotController(IUpdateReceiver receiver)
	{
		_receiver = receiver;
	}

	[HttpPost("GetUpdates")]
	public IActionResult Post([FromBody] Update update)
	{
		// Отправка новых сообщений от Telegram в цепочку обработчиков
		switch (update.Type)
		{
			case UpdateType.Message:
				_receiver.Receive(update.Message);
				break;
			case UpdateType.InlineQuery:
				_receiver.Receive(update.InlineQuery);
				break;
			case UpdateType.ChosenInlineResult:
				_receiver.Receive(update.ChosenInlineResult);
				break;
			case UpdateType.CallbackQuery:
				_receiver.Receive(update.CallbackQuery);
				break;
			case UpdateType.EditedMessage:
				_receiver.Receive(update.EditedMessage);
				break;
			case UpdateType.ChannelPost:
				_receiver.Receive(update.ChannelPost);
				break;
			case UpdateType.EditedChannelPost:
				_receiver.Receive(update.EditedChannelPost);
				break;
			case UpdateType.ShippingQuery:
				_receiver.Receive(update.ShippingQuery);
				break;
			case UpdateType.PreCheckoutQuery:
				_receiver.Receive(update.PreCheckoutQuery);
				break;
			case UpdateType.Poll:
				_receiver.Receive(update.Poll);
				break;
			case UpdateType.PollAnswer:
				_receiver.Receive(update.PollAnswer);
				break;
			case UpdateType.MyChatMember:
				_receiver.Receive(update.MyChatMember);
				break;
			case UpdateType.ChatMember:
				_receiver.Receive(update.ChatMember);
				break;
			case UpdateType.ChatJoinRequest:
				_receiver.Receive(update.ChatJoinRequest);
				break;
			case UpdateType.Unknown:
				break;
		}

		return Ok();
	}
}