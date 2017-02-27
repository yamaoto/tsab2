using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;

namespace Rikka.Tsab2.Core.Services
{
    public interface IBotService
    {
        Task<MessageFlow> Command(string command, MessageModel message);
        Task<MessageFlow> Message(string text, string state, MessageModel message);
        Task<MessageFlow> ChatCommand(string command, MessageModel message);
        Task<MessageFlow> ChatMessage(string text, string state, MessageModel message);
        Task<MessageFlow> PrivateCommand(string command, MessageModel message);
        Task<MessageFlow> PrivateMessage(string text, string state, MessageModel message);
    }

    public class BotService : IBotService
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly IBotApi BotApi;
        private readonly IBotAction[] _privateActions;
        private readonly IBotAction[] _chatActions;

        private readonly IBotAction _helpAction;
        private readonly IBotAction _mainAction;

        public BotService(IBotApi botapi, IBotActionsService botActionsService)
        {

            var actions = botActionsService.Actions.ToArray();
            _privateActions = actions.Where(w => w.Private).ToArray();
            _chatActions = actions.Where(w => w.Chat).ToArray();
            _helpAction = actions.Single(s => s.CommandName == "/help");
            _mainAction = actions.Single(s => s.CommandName == "/start");
            BotApi = botapi;
        }



        public async Task<MessageFlow> Command(string command, MessageModel message)
        {
            return message.From.Id == message.Chat.Id ? await PrivateCommand(command, message) : await ChatCommand(command, message);
        }

        public async Task<MessageFlow> Message(string text, string state, MessageModel message)
        {
            return message.From.Id == message.Chat.Id ? await PrivateMessage(text, state, message) : await ChatMessage(text, state, message);
        }

        public async Task<MessageFlow> ChatCommand(string command, MessageModel message)
        {
            var action = _chatActions.FirstOrDefault(f => f.CommandName.Equals(command, StringComparison.CurrentCultureIgnoreCase));
            if (action != null)
            {
                return await action.Command(command, message);
            }
            return new MessageFlow();
        }

        public async Task<MessageFlow> ChatMessage(string text, string state, MessageModel message)
        {
            var action = _chatActions.FirstOrDefault(f => f.States.Any(a => a == state));
            if (action != null)
            {
                return await action.Message(text, state, message);
            }
            return new MessageFlow();
        }

        public async Task<MessageFlow> PrivateCommand(string command, MessageModel message)
        {
            var action = _privateActions.FirstOrDefault(f => f.CommandName.Equals(command, StringComparison.CurrentCultureIgnoreCase));
            if (action != null)
            {
                return await action.Command(command, message);
            }
            return new MessageFlow();
        }

        public async Task<MessageFlow> PrivateMessage(string text, string state, MessageModel message)
        {
            var action = _privateActions.FirstOrDefault(f => f.States.Any(a => a == state));
            if (action != null)
            {
                return await action.Message(text, state, message);
            }
            return new MessageFlow();
        }
    }
}
