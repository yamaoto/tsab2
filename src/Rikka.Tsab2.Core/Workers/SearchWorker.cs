using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rikka.TelegamClasses.Models;
using Rikka.TelegramBotCore;
using Rikka.TelegramBotCore.Models;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.Workers
{
    public class SearchWorker:IWorker<SearchWorkerParameter>
    {
        private readonly ISearchService _searchService;
        private readonly IBotApi _botApi;
        private readonly IChatRepository _chatRepository;
        private readonly IBotHelper _botHelper;

        public SearchWorker(ISearchService searchService,IBotApi botApi,IChatRepository chatRepository, IBotHelper botHelper)
        {
            _searchService = searchService;
            _botApi = botApi;
            _chatRepository = chatRepository;
            _botHelper = botHelper;
        }
        public void Invoke(SearchWorkerParameter message)
        {
            _invoke(message).Wait();
        }

        private async Task _invoke (SearchWorkerParameter message)
        {
            await _search(message.ChatId, message.FromId, message.Expression);
        }

        private async Task _search(int chatId, int fromId, string expression)
        {
            var search = await _searchService.Search(chatId, expression, 20);
            var first = search.Next();
            if (first == null)
            {
                await _botApi.BotMethod(new SendMessageModel(chatId, "Похоже по такому запросу нету ничего..."));
                return;
            }
            await _botApi.BotMethod(new SendMessageModel(chatId, "Итак, вот что мне удалось найти по запросу #" + expression));
            //TODO: wait 300ms;
            var image = await _searchService.Download(first.ImageUrl);
            var markup = _botHelper.SingleReply(new[] { "Дальше", "Публикуй", "/cansel" });
            await _chatRepository.SetState(chatId, "NoState");
            await _botApi.BotMethod(new SendPhotoModel(chatId, image, replyMarkup: markup));
        }
        
    }

    public class SearchWorkerParameter
    {
        public SearchWorkerParameter()
        {
            
        }

        public SearchWorkerParameter(int chatId, int fromId, string expression)
        {
            ChatId = chatId;
            FromId = fromId;
            Expression = expression;
        }

        public int ChatId { get; set; }
        public int FromId { get; set; }
        public string Expression { get; set; }
    }
}
