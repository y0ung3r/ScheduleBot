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
    [Command(pattern: "/start")]
    public class SignUpSystem : SystemBase
    {
        public class SignUpStage
        {
            public long ChatId { get; }
            public Faculty ChoosedFaculty { get; set; }
            public Group ChoosedGroup { get; set; }

            public SignUpStage(long chatId)
            {
                ChatId = chatId;
            }
        }

        private readonly IScheduleParser _scheduleParser;
        private ICollection<Faculty> _faculties;
        private ICollection<Group> _groups;
        private readonly ICollection<SignUpStage> _stages;
        
        public SignUpSystem(IScheduleParser scheduleParser)
        {
            _scheduleParser = scheduleParser;

            _stages = new List<SignUpStage>();
        }

        public override async Task OnStartupAsync()
        {
            _faculties = await _scheduleParser.ParseFacultiesAsync();
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
                new SignUpStage(chatId)
            );

            var replyKeyboard = _faculties.ToReplyKeyboard
            (
                faculty => faculty.Abbreviation,
                oneTimeKeyboard: true
            );

            await Bot.Client.SendTextMessageAsync
            (
                chatId,
                "Перед началом работы необходимо произвести настройку бота.\n" +
                "Выберите факультет, к которому Вас нужно прикрепить:",
                replyMarkup: replyKeyboard
            );
        }

        public override async Task OnMessageReceivedAsync(Message message)
        {
            var chatId = message.Chat.Id;
            var stage = _stages.FirstOrDefault(stage => stage.ChatId.Equals(chatId));

            if (stage != null)
            {
                var messageText = message.Text;

                if (stage.ChoosedFaculty is null)
                {
                    await AskUserFacultyAsync(stage, messageText);
                }

                if (stage.ChoosedGroup is null)
                {
                    await AskUserGroupAsync(stage, messageText);
                }
            }
        }

        private async Task AskUserFacultyAsync(SignUpStage stage, string messageText)
        {
            stage.ChoosedFaculty = _faculties.FirstOrDefault(faculty => faculty.Abbreviation.Equals(messageText));

            if (stage.ChoosedFaculty != null)
            {
                _groups = await _scheduleParser.ParseGroupsAsync(stage.ChoosedFaculty.Id);

                var replyKeyboard = _groups.ToReplyKeyboard
                (
                    group => group.Title,
                    oneTimeKeyboard: true
                );

                await Bot.Client.SendTextMessageAsync
                (
                    stage.ChatId,
                    "Теперь выберите группу:",
                    replyMarkup: replyKeyboard
                );
            }
        }

        private async Task AskUserGroupAsync(SignUpStage stage, string messageText)
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
