using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rikka.Tsab2.Core
{
    public class BotException:Exception
    {
        public int ChatId { get; }

        public BotException(string message, int chatId):base(message)
        {
            ChatId = chatId;
        }
    }
}
