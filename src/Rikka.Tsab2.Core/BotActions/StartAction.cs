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
        public string Description => "�����������";

        public async Task<MessageFlow> Command(string command, MessageModel message)
        {
            var msg = $@"������, {message.From.FirstName}!
� @typical_saitama_adminBot. � ���� ������� �� ������ ��������� �������� �� �������� � ����������, ��� ����� ������ ����� /public ��� /help ��� ��������� ���� ��������";
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