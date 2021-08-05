using Microsoft.Extensions.Logging;
using ScheduleBot.Attributes;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Telegram.LongPolling.Interfaces;
using System;
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

            var nextDate = DateTime.Today
                                   .AddDays(1);

            var chatParameters = await _chatParametersService.GetChatParametersAsync(chatId);

            await _client.SendChatActionAsync
            (
                chatId,
                chatAction: ChatAction.Typing
            );

            var stringBuilder = new StringBuilder();

            if (chatParameters is not null)
            {
                var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, nextDate);

                stringBuilder.AppendLine($"<b>Расписание на {nextDate.ToShortDateString()}:</b>")
                             .AppendLine();

                if (studyDay.Lessons.Count > 0)
                {
                    foreach (var lesson in studyDay.Lessons)
                    {
                        stringBuilder.AppendLine($"<b>{lesson.Number} {lesson.Title}</b>");

                        if (!string.IsNullOrWhiteSpace(lesson.Type))
                        {
                            stringBuilder.Append($"<i>{lesson.Type}</i>");

                            if (!string.IsNullOrWhiteSpace(lesson.ClassroomNumber))
                            {
                                stringBuilder.Append($" <i>{lesson.ClassroomNumber}</i>");
                            }

                            stringBuilder.Append($" в {lesson.TimeRange}");
                        }

                        stringBuilder.AppendLine();

                        if (lesson.Teachers.Count > 0)
                        {
                            stringBuilder.AppendJoin(", ", lesson.Teachers);
                        }

                        stringBuilder.AppendLine()
                                     .AppendLine();
                    }
                }
                else
                {
                    stringBuilder.AppendLine("Пары отсутствуют");
                }
            }
            else
            {
                stringBuilder.AppendLine("Вы не настроили бота, чтобы использовать этот функционал. Используйте /bind, чтобы начать работу");
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
