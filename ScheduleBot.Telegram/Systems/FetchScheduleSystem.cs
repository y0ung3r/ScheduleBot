using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Extensions;
using ScheduleBot.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Telegram.Systems
{
    [Command(pattern: "/schedule")]
    public class FetchScheduleSystem : TelegramSystemBase
    {
        public class FetchScheduleStage
        {
            public long ChatId { get; }

            public FetchScheduleStage(long chatId)
            {
                ChatId = chatId;
            }
        }

        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;
        private readonly ICollection<FetchScheduleStage> _stages;

        public FetchScheduleSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
            _stages = new List<FetchScheduleStage>();
        }

        public override async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParameters(chatId);

            var stringBuilder = new StringBuilder();

            if (chatParameters != null)
            {
                var group = await _scheduleParser.ParseGroupAsync(chatParameters.FacultyId, chatParameters.GroupId, chatParameters.GroupTypeId);

                var dateTime = DateTime.Today;
                var studyDay = await _scheduleParser.ParseStudyDayAsync(group, dateTime);

                stringBuilder.AppendLine($"<b>Расписание на {dateTime.ToShortDateString()}:</b>")
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
                stringBuilder.AppendLine("Вы не настроили бота, чтобы использовать этот функционал.")
                             .AppendLine("Используйте /bind, чтобы начать работу");
            }

            await Client.SendTextMessageAsync
            (
                chatId,
                stringBuilder.ToString(),
                ParseMode.Html
            );

            await Client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        protected override async Task OnCommandReceivedAsync(Message command)
        {
            var chatId = command.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                _stages.Remove(stage);
            }

            _stages.Add
            (
                new FetchScheduleStage(chatId)
            );

            await Client.SendTextMessageAsync
            (
                chatId,
                "Выберите нужную учебную неделю, чтобы получить расписание:"
            );
        }
    }
}
