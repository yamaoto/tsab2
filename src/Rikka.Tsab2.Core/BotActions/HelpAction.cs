using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;

namespace Rikka.Tsab2.Core.BotActions
{
    [BotAction]
    public class HelpAction : IBotAction
    {
        private readonly IBotActionsService _botActionsService;

        public HelpAction(IBotActionsService botActionsService)
        {
            _botActionsService = botActionsService;
        }

        public bool Private { get; } = true;
        public bool Chat { get; } = true;
        public string[] States { get; } = new string[0];
        public string CommandName { get; } = "/help";
        public string Description { get; } = "Показать команды";

        public async Task<MessageFlow> Command(string command, MessageModel message)
        {
            var isPrivate = message.Chat.Id == message.From.Id;
            var msg = $"Итак, {message.From.FirstName}!\r\nВот команды на которые меня обучил @yamaoto:\r\n";
            var actions = _botActionsService.Actions
                .Where(w => w.CommandName != null && (isPrivate && w.Private) || (!isPrivate && w.Chat))
                .OrderBy(o => o.CommandName);
            msg = actions.Aggregate(msg, (current, action) => current + $"{action.CommandName} - {action.Description}\r\n");
            return new MessageFlow(MessageFlowItem.GetMessage(msg));
        }       

        public async Task<MessageFlow> Message(string text, string state, MessageModel message)
        {
            return null;
        }        

    }
}
