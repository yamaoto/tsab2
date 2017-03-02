using System;
using System.Linq;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Context.Entities;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.BotActions
{
    [BotAction]
    public class SearchSetupAction : BaseBotAction,IBotAction
    {
        private readonly ISearchService _searchService;
        private readonly IBotHelper _botHelper;
        private readonly IChatRepository _chatRepository;
        private readonly ISearchEngineRepository _searchEngineRepository;
        public bool Private { get; } = true;
        public bool Chat { get; } = true;
        
        public string CommandName { get; } = "/searchsetup";
        public string Description { get; } = "Настройка поисковых систем";

        


        public SearchSetupAction(ISearchService searchService,IBotHelper botHelper,IChatRepository chatRepository,ISearchEngineRepository searchEngineRepository)
        {
            _searchService = searchService;
            _botHelper = botHelper;
            _chatRepository = chatRepository;
            _searchEngineRepository = searchEngineRepository;

            Register(state=> state.SearchSetupConfigureEngine, ChooseEngine);
            Register(state=>state.SearchSetupConfigureEngine, ConfigureEngine);
            Register(state=>state.SearchSetupConfigureEngineSpec, ConfigureEngineSpec);
        }

        public async Task<MessageFlow> Command(string command, MessageModel message)
        {
            var engines = _searchService.Engines.Select(s => s.EngineName).ToArray();
            await _chatRepository.SetState(message.Chat.Id, BotStates.SearchSetupChooseEngine);
            return new MessageFlow().Message("Какую поисковую службу?",
                replyMarkup: _botHelper.SingleReply(engines));
        }



        public async Task<MessageFlow> ChooseEngine(string text, MessageModel message)
        {            
            var engines = _searchService.Engines.Select(s => s.EngineName).ToArray();
            if (engines.Contains(text))
            {
                await _chatRepository.SetState(message.Chat.Id, BotStates.SearchSetupChooseEngine,text);
                return new MessageFlow().Message("Настроить по умолчанию или специализированно?",
                    replyMarkup: _botHelper.SingleReply(new[] { "по умолчанию", "специализированно" }));
            }
            else
            {
                return new MessageFlow().Message("Походу ты в чем то ошибся");
            }
        }

        public async Task<MessageFlow> ConfigureEngine(string text, MessageModel message)
        {
            var name =await  _chatRepository.GetStateParams(message.Chat.Id);
            var engineInstance = _searchService.Engines.First(f => f.EngineName == name);
            var engine = await _searchEngineRepository.GetEngine(name, message.Chat.Id);
            if (engine == null)
            {
                engine = new SearchEngine(name,message.Chat.Id);
                await _searchEngineRepository.Insert(engine);
            }
            var defaults = new[] { "по умолчанию ", "умолчанию", "да" };
            var special = new[] { " специализированно", "нет" };
            if (defaults.Contains(text))
            {
                engine.Token = engineInstance.DefaultToken;
                engine.Additional = engineInstance.DefaultAdditional;
                await _searchEngineRepository.Update(engine);
                return new MessageFlow()
                    .Message("Ok!");
            } else if (special.Contains(text))
            {
                await _chatRepository.SetState(message.Chat.Id, BotStates.SearchSetupConfigureEngineSpec, name);
                return new MessageFlow()
                    .Message("Хошорошо..")
                    .Message(engineInstance.ConfigureUrl)
                    .Message(engineInstance.ConfigureDescription);
            }
            else
            {
                return new MessageFlow().Message("Что за дичь ты мне тут втираешь?");
            }
        }

        public async Task<MessageFlow> ConfigureEngineSpec(string text, MessageModel message)
        {
            var name = await _chatRepository.GetStateParams(message.Chat.Id);
            var engine = await _searchEngineRepository.GetEngine(name, message.Chat.Id);
            if (engine == null)
            {
                engine = new SearchEngine(name, message.Chat.Id);
                await _searchEngineRepository.Insert(engine);
            }

            throw new NotImplementedException();
        }
    }
}
