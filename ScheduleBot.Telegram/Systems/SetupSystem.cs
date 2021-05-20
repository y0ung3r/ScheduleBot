using ScheduleBot.Attributes;
using ScheduleBot.Data.Models;
using ScheduleBot.Extensions;
using ScheduleBot.Parser.Interfaces;
using ScheduleBot.Parser.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Systems
{
    [Command(pattern: "/setup")]
    public class SetupSystem : SystemBase
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

        private readonly IScheduleParser _scheduleParser;
        private ICollection<Faculty> _faculties;
        private ICollection<Group> _groups;
        private readonly ICollection<SetupStage> _stages;
        
        public SetupSystem(IScheduleParser scheduleParser)
        {
            _scheduleParser = scheduleParser;

            _stages = new List<SetupStage>();
        }

        public override async Task OnStartupAsync()
        {
            _faculties = await _scheduleParser.ParseFacultiesAsync();
        }

        public override async Task OnUpdateReceivedAsync(Update update)
        {
            if (update.CallbackQuery != null)
            {
                var chatId = update.CallbackQuery.Message.Chat.Id;
                var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

                if (stage != null)
                {
                    var messageText = update.CallbackQuery.Data;

                    if (stage.ChoosedFaculty is null)
                    {
                        await AskUserFacultyAsync(stage, messageText);
                    }

                    if (_groups != null && stage.ChoosedGroup is null)
                    {
                        await AskUserGroupAsync(stage, messageText);
                    }
                }

                await Bot.Client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }

        public override async Task OnCommandReceivedAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                _stages.Remove(stage);
            }

            _stages.Add
            (
                new SetupStage(chatId)
            );

            var replyKeyboard = _faculties.ToInlineKeyboard
            (
                faculty => faculty.Abbreviation,
                chunkSize: 2
            );

            await Bot.Client.SendTextMessageAsync
            (
                chatId,
                "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: replyKeyboard
            );
        }

        private async Task AskUserFacultyAsync(SetupStage stage, string messageText)
        {
            stage.ChoosedFaculty = _faculties.FirstOrDefault(faculty => faculty.Abbreviation.Equals(messageText));

            if (stage.ChoosedFaculty != null)
            {
                _groups = await _scheduleParser.ParseGroupsAsync(stage.ChoosedFaculty.Id);

                var replyKeyboard = _groups.ToInlineKeyboard
                (
                    group => group.Title,
                    chunkSize: 2
                );

                await Bot.Client.SendTextMessageAsync
                (
                    stage.ChatId,
                    "Теперь выберите группу:",
                    replyMarkup: replyKeyboard
                );
            }
        }

        private async Task AskUserGroupAsync(SetupStage stage, string messageText)
        {
            stage.ChoosedGroup = _groups.FirstOrDefault(group => group.Title.Equals(messageText));

            if (stage.ChoosedGroup != null)
            {
                var userSchedule = await Bot.UnitOfWork.UserSchedules.FindUserSchedule(stage.ChatId);

                if (userSchedule is null)
                {
                    userSchedule = new UserSchedule()
                    {
                        ChatId = stage.ChatId,
                        FacultyId = stage.ChoosedFaculty.Id,
                        GroupId = stage.ChoosedGroup.Id,
                        GroupTypeId = stage.ChoosedGroup.TypeId
                    };

                    await Bot.UnitOfWork.UserSchedules.AddAsync(userSchedule);
                }
                else
                {
                    userSchedule.FacultyId = stage.ChoosedFaculty.Id;
                    userSchedule.GroupId = stage.ChoosedGroup.Id;
                    userSchedule.GroupTypeId = stage.ChoosedGroup.TypeId;
                }

                await Bot.UnitOfWork.SaveChangesAsync();

                await Bot.Client.SendTextMessageAsync
                (
                    stage.ChatId,
                    "Отлично, настройка завершена!\nТеперь Вы будете получать расписание с учетом сохраненных параметров"
                );

                _stages.Remove(stage);
            }
        }
    }
}
