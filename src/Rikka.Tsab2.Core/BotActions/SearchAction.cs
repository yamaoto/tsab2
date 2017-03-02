using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Core.Workers;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.BotActions
{
    [BotAction]
    public class SearchAction : BaseBotAction,IBotAction
    {
        private readonly IChatRepository _chatRepository;
        private readonly ISearchService _searchService;
        private readonly ISearchEngineRepository _searchEngineRepository;
        private readonly IBotApi _botApi;
        private readonly IWorkerService _workerService;
        private readonly IBotHelper _botHelper;

        public SearchAction(IChatRepository chatRepository, ISearchService searchService, ISearchEngineRepository searchEngineRepository, IBotApi botApi, IWorkerService workerService, IBotHelper botHelper)
        {
            _chatRepository = chatRepository;
            _searchService = searchService;
            _searchEngineRepository = searchEngineRepository;
            _botApi = botApi;
            _workerService = workerService;
            _botHelper = botHelper;

            Register(state=>state.SearchChooseTag,_chooseTag);
            Register(state=>state.SearchPickResults,_steps);
        }
        public async Task<MessageFlow> Command(string command, TelegamClasses.Models.MessageModel message)
        {
            if(await _searchService.CheckEngines(message.Chat.Id))
            {
                return new MessageFlow(new SendMessageModel(message.Chat.Id, "Похоже что у тебя еще не настроены поисковые системы. Сделай для этого /searchsetup"));
            }
            var popular = await _searchService._getPopularTags(message.Chat.Id);
            await _chatRepository.SetState(message.Chat.Id, BotStates.SearchChooseTag);
            return new MessageFlow()
                .Message("Давай поищем, только скажи по какому тегу?", replyMarkup:_botHelper.SingleReply(popular));
        }

        private async Task<MessageFlow> _steps(string text, MessageModel message)
        {
            if (!_searchService.Results.ContainsKey(message.Chat.Id))
            {
                await _chatRepository.SetState(message.Chat.Id, BotStates.NoState);
                return new MessageFlow()
                    .Message("А ты уверен что производил поиск? Если что, напиши /search для этого...");
            }
            var show = new[] { "покажи", "инфо", "сведения", "инфа", "показать инфу", "показать инфо", "показать" };
            var post = new[] { "публикуй", "действуй", "в отложку", "вк", "в вк", "паблик", "в паблик", "опубликовать фото", "опубликовать" };
            var next = new[] { "дальше", "далее", "еще", "следующий", "следующая", "показать следующее" };
            var current = _searchService.Results[message.Chat.Id].Current;
            if (current == null)
            {
                await _chatRepository.SetState(message.Chat.Id, BotStates.NoState);
                return new MessageFlow().Message("На этом все");
            }
            if (show.Any(a => a == text))
            {
                return new MessageFlow()
                    .Message(current.ItemUrl)
                    .Message(current.Description);
            } else if (post.Any(a => a == text))
            {
                throw new NotImplementedException();

            } else if (next.Any(a => a == text))
            {
                return await _pickResults(message);
            }
            else
            {
                return new MessageFlow()
                    .Message("Что-то я тебя не понял");
            }
        }
        private async Task<MessageFlow> _pickResults(MessageModel message)
        {                           
            var search = _searchService.Results[message.Chat.Id];

            var item = search.Next();
            if (item == null)
            {                
                await _chatRepository.SetState(message.Chat.Id, BotStates.NoState);
                return new MessageFlow()
                    .Message("Это все! Действительно все. Больше нету картинок");
            }
            var image = await _searchService.Download(item.ImageUrl);;
            return new MessageFlow()
                .Photo(image, replyMarkup: _botHelper.SingleReply(new[] {"Дальше", "Публикуй", "/cansel"}))
                .Message($"Найдено в '{item.Engine}', рейтинг {item.Score}");
        }

        private async Task<MessageFlow> _chooseTag(string tag, MessageModel message)
        {
            if (tag.StartsWith("#"))
                tag = tag.Substring(1);
            if (tag.Contains(" "))
            {
                return new MessageFlow()
                    .Message("Что-то не похоже на тег...");
            }
            await _chatRepository.SetState(message.Chat.Id, BotStates.NoState);

            _workerService.PushMessage(new JobMessage("search",new SearchWorkerParameter(message.Chat.Id,message.From.Id,tag)));

            return new MessageFlow()
                .Message("Подожди некоторое время...", replyMarkup: _botHelper.Hide())
                .Sticker("BQADBAADTAUAAqKYZgABfaNgr6BIuFIC");
        }
        


        public bool Private { get; } = true;
        public bool Chat { get; } = true;
        public string CommandName { get; } = "/search";
        public string Description { get; } = "поиск картинок";
    }
}