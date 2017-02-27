using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;

namespace Rikka.Tsab2.Core.BotActions
{
    [BotAction]
    public class StartAction : IBotAction
    {
        public bool Private => true;
        public bool Chat => true;
        public string[] States => new[] { "NoState" };
        public string CommandName => "/start";
        public string Description => "Приветствие";

        public async Task<MessageFlow> Command(string command, MessageModel message)
        {
            var msg = $@"Привет, {message.From.FirstName}!
Я @typical_saitama_adminBot. С моей помощью ты можешь проверять картинки на загрузку в сообщества, для этого просто введи /public или /help для получения всех подказок";
            var result = new MessageFlow()
            {
                MessageFlowItem.GetMessage(msg),
                MessageFlowItem.GetSticker("BQADBAADtwMAAqKYZgABJFsIZLA51N0C")
            };
            return result;
        }

        
        public async Task<MessageFlow> Message(string text, string state, MessageModel message)
        {
            return new MessageFlow(MessageFlowItem.GetMessage(".-."));
        }
    }
}