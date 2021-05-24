using ScheduleBot.Attributes;
using ScheduleBot.Data.Interfaces;
using ScheduleBot.Data.Models;
using ScheduleBot.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/bind")]
    public class BindSystem : SystemBase
    {
        public class SetupStage
        {
            public long ChatId { get; }
            public Faculty ChoosedFaculty { get; set; }
            public Group ChoosedGroup { get; set; }

            public SetupStage(long chatId)
            {
                ChatId = chatId;
            }
        }

        private readonly IBotUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;
        private readonly ICollection<SetupStage> _stages;
        private ICollection<Faculty> _faculties;
        private ICollection<Group> _groups;

        public BindSystem(IBotUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
            _stages = new List<SetupStage>();
        }

        private async Task AskUserFacultyAsync(ITelegramBotClient client, SetupStage stage, string callbackData)
        {
            stage.ChoosedFaculty = _faculties.FirstOrDefault(faculty => faculty.Abbreviation.Equals(callbackData));

            if (stage.ChoosedFaculty != null)
            {
                _groups = await _scheduleParser.ParseGroupsAsync(stage.ChoosedFaculty.Id);

                var inlineKeyboard = _groups.ToInlineKeyboard
                (
                    group => group.Title,
                    columnsCount: 3
                );

                await client.SendTextMessageAsync
                (
                    stage.ChatId,
                    "Теперь выберите группу:",
                    replyMarkup: inlineKeyboard
                );
            }
        }

        private async Task AskUserGroupAsync(ITelegramBotClient client, SetupStage stage, string callbackData)
        {
            stage.ChoosedGroup = _groups.FirstOrDefault(group => group.Title.Equals(callbackData));

            if (stage.ChoosedGroup != null)
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

                await client.SendTextMessageAsync
                (
                    stage.ChatId,
                    "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
                );

                _stages.Remove(stage);
            }
        }

        public override async Task OnCallbackQueryReceivedAsync(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                var callbackData = callbackQuery.Data;

                if (stage.ChoosedFaculty is null)
                {
                    await AskUserFacultyAsync(client, stage, callbackData);
                }

                if (_groups != null && stage.ChoosedGroup is null)
                {
                    await AskUserGroupAsync(client, stage, callbackData);
                }
            }

            await client.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        public override async Task OnCommandReceivedAsync(ITelegramBotClient client, Message command)
        {
            var chatId = command.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                _stages.Remove(stage);
            }

            _stages.Add
            (
                new SetupStage(chatId)
            );

            _faculties = await _scheduleParser.ParseFacultiesAsync();

            var inlineKeyboard = _faculties.ToInlineKeyboard
            (
                faculty => faculty.Abbreviation,
                columnsCount: 2
            );

            await client.SendTextMessageAsync
            (
                chatId,
                "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: inlineKeyboard
            );
        }
    }
}
