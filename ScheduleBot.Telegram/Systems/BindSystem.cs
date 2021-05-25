using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.Models;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using ScheduleBot.Telegram.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Telegram.Systems
{
    [Command(pattern: "/bind")]
    public class BindSystem : TelegramSystemBase
    {
        public class BindStage
        {
            public long ChatId { get; }
            public Faculty ChoosedFaculty { get; set; }
            public Group ChoosedGroup { get; set; }

            public BindStage(long chatId)
            {
                ChatId = chatId;
            }
        }

        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;
        private readonly ICollection<BindStage> _stages;
        private ICollection<Faculty> _faculties;
        private ICollection<Group> _groups;

        public BindSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
            _stages = new List<BindStage>();
        }

        private bool TryRememberFaculty(BindStage stage, string facultyAbbreviation)
        {
            stage.ChoosedFaculty = _faculties.FirstOrDefault(faculty => faculty.Abbreviation.Equals(facultyAbbreviation));
            return stage.ChoosedFaculty != null;
        }

        private bool TryRememberGroup(BindStage stage, string groupTitle)
        {
            stage.ChoosedGroup = _groups.FirstOrDefault(group => group.Title.Equals(groupTitle));
            return stage.ChoosedGroup != null;
        }

        private async Task AskFacultyAsync(ChatId chatId)
        {
            _faculties = await _scheduleParser.ParseFacultiesAsync();

            var inlineKeyboard = _faculties.ToInlineKeyboard
            (
                faculty => faculty.Abbreviation,
                columnsCount: 2
            );

            await Client.SendTextMessageAsync
            (
                chatId,
                "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: inlineKeyboard
            );
        }

        private async Task AskGroupAsync(BindStage stage)
        {
            _groups = await _scheduleParser.ParseGroupsAsync(stage.ChoosedFaculty.Id);

            var inlineKeyboard = _groups.ToInlineKeyboard
            (
                group => group.Title,
                columnsCount: 3
            );

            await Client.SendTextMessageAsync
            (
                stage.ChatId,
                "Теперь выберите группу:",
                replyMarkup: inlineKeyboard
            );
        }

        private async Task SaveChoiceAsync(BindStage stage)
        {
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParameters(stage.ChatId);

            if (chatParameters is null)
            {
                chatParameters = new ChatParameters()
                {
                    ChatId = stage.ChatId,
                    FacultyId = stage.ChoosedFaculty.Id,
                    GroupId = stage.ChoosedGroup.Id,
                    GroupTypeId = stage.ChoosedGroup.TypeId
                };

                await _unitOfWork.ChatParameters.AddAsync(chatParameters);
            }
            else
            {
                chatParameters.FacultyId = stage.ChoosedFaculty.Id;
                chatParameters.GroupId = stage.ChoosedGroup.Id;
                chatParameters.GroupTypeId = stage.ChoosedGroup.TypeId;
            }

            await _unitOfWork.SaveChangesAsync();

            await Client.SendTextMessageAsync
            (
                stage.ChatId,
                "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
            );

            _stages.Remove(stage);
        }

        public override async Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                var callbackData = callbackQuery.Data;

                if (stage.ChoosedFaculty is null && TryRememberFaculty(stage, callbackData))
                {
                    await AskGroupAsync(stage);
                }

                if (_groups != null && stage.ChoosedGroup is null && TryRememberGroup(stage, callbackData))
                {
                    await SaveChoiceAsync(stage);
                }
            }

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
                new BindStage(chatId)
            );

            await AskFacultyAsync(chatId);
        }
    }
}
