using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore.Models;

namespace Rikka.Tsab2.Core.BotActions
{
    public abstract class BaseBotAction
    {
        public string[] States => StateHandlers.Keys.ToArray();
        public delegate Task<MessageFlow> StateHandler(string text, MessageModel message);
        protected readonly Dictionary<string, StateHandler> StateHandlers;
        protected readonly BotStates BotStates;

        protected BaseBotAction()
        {
            BotStates = new BotStates();
            StateHandlers = new Dictionary<string, StateHandler>();
        }

        protected void Register(Expression<Func<BotStates,string>> expression, StateHandler handler)
        {
            var expressionBody = expression.Compile();
            var state = expressionBody.Invoke(BotStates);
            this.StateHandlers.Add(state,handler);
        }

        public virtual async Task<MessageFlow> Message(string text, string state, MessageModel message)
        {
            if (StateHandlers.ContainsKey(state))
            {
                return await StateHandlers[state].Invoke(text, message);
            }
            else
            {
                return new MessageFlow()
                    .Message("o_O");
            }
        }
    }

    public class BotStates
    {
        public string NoState => "NoState";

        public string SearchSetupChooseEngine => "searchsetup-choose-engine";
        public string SearchSetupConfigureEngine => "searchsetup-configure-engine";
        public string SearchSetupConfigureEngineSpec => "searchsetup-configure-engine-spec";
        public string SearchChooseTag => "search-choose-tag";
        public string SearchPickResults => "search-pick-results";
        public static BotStates Get()
        {
            return new BotStates();
        }
    }
}