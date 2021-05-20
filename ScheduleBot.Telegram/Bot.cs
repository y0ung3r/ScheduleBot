using ScheduleBot.Data.Interfaces;
using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ScheduleBot
{
    public class Bot : IBot
    {
        private readonly string _token;
        private readonly IReadOnlyCollection<ISystem> _systems;

        public TelegramBotClient Client { get; }
        public IBotUnitOfWork UnitOfWork { get; }

        public Bot(string token, IReadOnlyCollection<ISystem> systems, IBotUnitOfWork unitOfWork)
        {
            _token = token;
            _systems = systems;

            Client = new TelegramBotClient(_token);
            Client.OnUpdate += OnUpdateReceived;
            Client.OnMessage += OnMessageReceived;

            UnitOfWork = unitOfWork;
        }

        private void OnUpdateReceived(object sender, UpdateEventArgs eventArgs)
        {
            OnUpdateReceivedAsync(eventArgs.Update).GetAwaiter()
                                                   .GetResult();
        }

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            OnMessageReceivedAsync(eventArgs.Message).GetAwaiter()
                                                     .GetResult();
        }

        private async Task OnUpdateReceivedAsync(Update update)
        {
            Console.WriteLine($"Bot received an update with the identifier: {update.Id}");

            foreach (var system in _systems)
            {
                await system.OnUpdateReceivedAsync(update);
            }
        }

        private async Task OnMessageReceivedAsync(Message message)
        {
            Console.WriteLine($"Bot received a message on chat with the identifier: {message.Chat.Id}");

            foreach (var system in _systems)
            {
                var messageText = message.Text;
                var systemType = system.GetType();

                if (system.MessageIsCommand(messageText))
                {
                    await system.OnCommandReceivedAsync(message);

                    Console.WriteLine($"Command \"{messageText}\" executed on the system: \"{systemType}\"");
                }
                else
                {
                    await system.OnMessageReceivedAsync(message);

                    Console.WriteLine($"A message transfer to the system: \"{systemType}\"");
                }
            }
        }

        public void Run()
        {
            foreach (var system in _systems)
            {
                system.Initialize(bot: this);

                system.OnStartupAsync()
                      .GetAwaiter()
                      .GetResult();
            }

            Client.StartReceiving();

            Console.WriteLine("Bot is running");
            Console.ReadKey();

            Client.StopReceiving();
        }
    }
}
