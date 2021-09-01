using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Commands
{
    [CommandText("/tomorrow")]
    public class TomorrowCommand : TelegramCommandBase
    {
        private readonly ILogger<ScheduleCommand> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;

        public TomorrowCommand(ILogger<ScheduleCommand> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
            IChatParametersService chatParametersService)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
        }

        protected override async Task HandleAsync(Message message, string[] arguments, RequestDelegate nextHandler)
        {
            var chatId = message.Chat.Id;
            var nextDate = DateTime.Today.AddDays(1);

            var stringBuilder = new StringBuilder();

            if (!nextDate.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                await _client.SendChatActionAsync
                (
                    chatId,
                    chatAction: ChatAction.Typing
                );

                var group = default(Group);
                var incomingGroupTitle = arguments.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(incomingGroupTitle))
                {
                    group = await _scheduleParser.ParseGroupAsync(incomingGroupTitle);

                    if (group is null)
                    {
                        stringBuilder.AppendLine($"Группа под названием \"{incomingGroupTitle}\" не найдена. Пожалуйста, уточните запрос");
                    }
                }
                else
                {
                    var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

                    if (chatParameters is not null)
                    {
                        group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                    }
                    else
                    {
                        stringBuilder.AppendLine("Вы не настроили бота, чтобы использовать этот функционал. Используйте /bind, чтобы начать работу");
                    }
                }

                if (group is not null)
                {
                    var studyDay = await _scheduleParser.ParseStudyDayAsync(group, nextDate);

                    var html = studyDay.ToHTML();
                    stringBuilder.AppendLine(html);
                }
            }
            else
            {
                stringBuilder.AppendLine("Завтра выходной день. Используйте /schedule, чтобы получить расписание на следующую неделю");
            }

            await _client.SendTextMessageAsync
            (
                chatId,
                text: stringBuilder.ToString(),
                parseMode: ParseMode.Html
            );

            _logger?.LogInformation("Tomorrow command processed");
        }
    }
}
