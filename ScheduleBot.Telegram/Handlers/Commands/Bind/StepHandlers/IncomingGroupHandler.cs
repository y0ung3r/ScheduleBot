﻿using System.Linq;
using System.Threading.Tasks;
using BotFramework.Handlers.StepHandlers;
using Microsoft.Extensions.Logging;
using ScheduleBot.Domain.Interfaces;
using ScheduleBot.Parser.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Telegram.Handlers.Commands.Bind.StepHandlers
{
    internal sealed class IncomingGroupHandler : StepHandlerBase<Update, Update>
    {
        private readonly ILogger<IncomingGroupHandler> _logger;
        private readonly ITelegramBotClient _client;
        private readonly IScheduleParser _scheduleParser;
        private readonly IChatParametersService _chatParametersService;
        
        public IncomingGroupHandler(ILogger<IncomingGroupHandler> logger, ITelegramBotClient client, IScheduleParser scheduleParser,
            IChatParametersService chatParametersService)
        {
            _logger = logger;
            _client = client;
            _scheduleParser = scheduleParser;
            _chatParametersService = chatParametersService;
        }
        
        public override async Task HandleAsync(Update request, Update response)
        {
            var responseCallbackQuery = response.CallbackQuery;
            var message = responseCallbackQuery.Message;
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            
            var faculties = await _scheduleParser.ParseFacultiesAsync();
            var facultyAbbreviation = request.CallbackQuery.Data;
            var faculty = faculties.FirstOrDefault
            (
                faculty => faculty.TitleAbbreviation.Equals(facultyAbbreviation)
            );

            if (faculty is not null)
            {
                var groups = await _scheduleParser.ParseGroupsAsync(faculty.Id);
                var groupTitle = responseCallbackQuery.Data;
                var group = groups.FirstOrDefault
                (
                    group => group.Title.Equals(groupTitle)
                );

                if (group is not null)
                {
                    await _client.SendChatActionAsync(chatId, ChatAction.Typing);

                    await _chatParametersService.SaveChatParametersAsync
                    (
                        chatId,
                        faculty.Id,
                        group.Id,
                        group.TypeId
                    );

                    await _client.DeleteMessageAsync(chatId, messageId);

                    await _client.SendTextMessageAsync
                    (
                        chatId,
                        text: "Настройка завершена. Теперь Вы будете получать расписание с учетом сохраненных параметров"
                    );
                }
            }
            
            await _client.AnswerCallbackQueryAsync(responseCallbackQuery.Id);
        }
        
        public override bool CanHandle(Update request, Update response)
        {
            return request is not null && response is not null;
        }
    }
}