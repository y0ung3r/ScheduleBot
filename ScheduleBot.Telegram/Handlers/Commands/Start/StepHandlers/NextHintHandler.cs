using System.Threading.Tasks;
using BotFramework.Handlers.StepHandlers;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Handlers.Commands.Start.StepHandlers
{
	public class NextHintHandler : StepHandlerBase<Update, Update>
	{
		public override Task HandleAsync(Update request, Update response)
		{
			throw new System.NotImplementedException();
		}
	}
}