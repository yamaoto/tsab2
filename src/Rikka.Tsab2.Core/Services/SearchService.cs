using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Rikka.Tsab2.Core.Services.SearchEngine;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.Services
{
    public interface ISearchService
    {
        ISearchEngine[] Engines { get; set; }
        Task<SearchResult> Search(int chatId,string tag, int totalForEach, DateTime? after=null);

        IReadOnlyDictionary<int, SearchResult> Results { get; }
        Task<byte[]> Download(string imageUrl);
        Task<bool> CheckEngines(int chatId);
    }

    public class SearchService : ISearchService
    {
        private readonly ISearchEngineRepository _searchEngineRepository;
        public ISearchEngine[] Engines { get; set; }

        private readonly Dictionary<int, SearchResult> _results =
            new Dictionary<int, SearchResult>();

        public SearchService(ISearchEngineRepository searchEngineRepository)
        {
            _searchEngineRepository = searchEngineRepository;
            Engines = GetEngines();
        }

        public async Task<SearchResult> Search(int chatId, string tag, int totalForEach, DateTime? after)
        {
            var items = new List<ISearchResultItem>();
            foreach (var engine in Engines)
            {
                IEnumerable<ISearchResultItem> inner;
                try
                {
                    var info = await _searchEngineRepository.GetEngine(engine.EngineName, chatId);
                    if (info == null)
                        continue;
                    inner = await engine.Search(tag, totalForEach, after, info.Token, info.Additional);
                }
                catch
                {
                    continue;
                }
                items.AddRange(inner);
            }
            var sorted = items.OrderByDescending(d => d.Score).ToArray();
            var result = new SearchResult(sorted);
            _results[chatId] = result;
            return result;
        }

        public static ISearchEngine[] GetEngines()
        {
            return new ISearchEngine[]
            {
             new TumblrSearchEngine(),    
            };
        }

        public IReadOnlyDictionary<int, SearchResult> Results => _results;

        public async Task<byte[]> Download(string imageUrl)
        {
            var client = new HttpClient();
            return await client.GetByteArrayAsync(imageUrl);
        }

        public async Task<bool> CheckEngines(int chatId)
        {
            var engines = await _searchEngineRepository.GetEngines(chatId);
            return engines.Any();

        }
    }

    public interface ISearchEngine
    {
        string EngineName { get; }
        Task<IEnumerable<ISearchResultItem>> Search(string tag, int count, DateTime? after, string token, string additional);

        string DefaultToken { get; }
        string DefaultAdditional { get; }
        string ConfigureUrl { get; }
        string ConfigureDescription { get; }
    }

    public class SearchResult
    {
        public SearchResult(ISearchResultItem[]  items)
        {
            Items = items;
        }

        public ISearchResultItem Next()
        {
            var item = _getCurrent();
            if (item == null)
                return null;
            Position++;
            return item;
        }
        public ISearchResultItem[] Items { get; private set; }
        public int Position { get; set; }
        public ISearchResultItem Current => _getCurrent();

        private ISearchResultItem _getCurrent()
        {
            if (Items.Length >= Position)
                return null;
            var item = Items[Position];
            return item;
        }
    }
    public interface ISearchResultItem
    {
        string ItemUrl { get; set; }
        string ImageUrl { get; set; }
        string Description { get; set; }
        string[] Tags { get; set; }
        string Engine { get; set; }
        string Group { get; set; }
        int Score { get; set; }
    }
}
