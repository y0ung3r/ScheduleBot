using ScheduleBot.Data.Models;
using ScheduleBot.Data.UnitOfWorks.Interfaces;
using ScheduleBot.Domain.DTO;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using System.Threading.Tasks;

namespace ScheduleBot.Domain
{
    public class ChatParametersService : IChatParametersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleParser _scheduleParser;

        public ChatParametersService(IUnitOfWork unitOfWork, IScheduleParser scheduleParser)
        {
            _unitOfWork = unitOfWork;
            _scheduleParser = scheduleParser;
        }

        public async Task<ChatParametersDTO> GetChatParametersAsync(long chatId)
        {
            var chatParametersDTO = default(ChatParametersDTO);
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParametersAsync(chatId);

            if (chatParameters is not null)
            {
                var facultyId = chatParameters.FacultyId;
                var faculty = await _scheduleParser.ParseFacultyAsync(facultyId);
                var group = await _scheduleParser.ParseGroupAsync(facultyId, chatParameters.GroupId, chatParameters.GroupTypeId);

                chatParametersDTO = new ChatParametersDTO()
                {
                    ChatId = chatId,
                    FacultyId = faculty.Id,
                    GroupId = group.Id,
                    GroupTypeId = group.TypeId,
                    FacultyTitleWithoutTag = faculty.TitleWithoutFacultyTag,
                    GroupTitle = group.Title
                };
            }

            return chatParametersDTO;
        }

        public async Task SaveChatParametersAsync(long chatId, int facultyId, int groupId, int groupTypeId)
        {
            var chatParameters = await _unitOfWork.ChatParameters.FindChatParametersAsync(chatId);

            if (chatParameters is null)
            {
                chatParameters = new ChatParameters()
                {
                    ChatId = chatId,
                    FacultyId = facultyId,
                    GroupId = groupId,
                    GroupTypeId = groupTypeId
                };

                await _unitOfWork.ChatParameters.AddAsync(chatParameters);
            }
            else
            {
                chatParameters.FacultyId = facultyId;
                chatParameters.GroupId = groupId;
                chatParameters.GroupTypeId = groupTypeId;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
