﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Rikka.Tsab2.Core.Services.SearchEngine
{
    public class TumblrSearchEngine : ISearchEngine
    {
        private readonly HttpClient _client;

        public TumblrSearchEngine()
        {
            _client = new HttpClient();
        }

        public string EngineName { get; } = "Tumblr";
        public async Task<IEnumerable<ISearchResultItem>> Search(string tag, int total, DateTime? after, string token, string additional)
        {
            var resultItems = new List<TumblrSearchResultItem>();
            var count = 0;
            int? timestamp = null;
            var tryCount = 0;
            const int maxTryCount = 5;
            while (count < total)
            {
                TumblrSearchResultItem[] items;
                try
                {
                    items = await _search(tag, after, timestamp, token,additional);
                }
                catch
                {
                    if (tryCount < maxTryCount)
                    {
                        tryCount++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                timestamp = items.Last()?.Timestamp;
                count += items.Length;
                resultItems.AddRange(items);
            }
            return resultItems;
        }

        private async Task<TumblrSearchResultItem[]> _search(string tag, DateTime? after, int? timestamp, string token, string additional)
        {
            var url = $"http://api.tumblr.com/v2/tagged?tag={tag}&api_key={token}";
            if (timestamp.HasValue)
                url += "&before=" + timestamp.Value;
            var resultData = await _client.GetStringAsync(url);

            dynamic json = JObject.Parse(resultData);

            if (json?.meta?.msg != "OK")
            {
                return null;
            }
            var result = new List<TumblrSearchResultItem>();
            foreach (var item in json.response)
            {
                if (item.type != "photo")
                    continue;
                var creationDate = DateTime.Parse((string)item.date);
                if (after.HasValue && creationDate < after)
                    continue;
                var photos = (JArray)item.photos;
                var group = photos.Count > 1 ? Guid.NewGuid().ToString("N") : null;
                var tags = new List<string>();
                foreach (string photoTag in item.tags)
                {
                    tags.Add(photoTag);
                }
                foreach (var photo in item.photos)
                {

                    var resultItem = new TumblrSearchResultItem()
                    {
                        ItemUrl = item.post_url,
                        ImageUrl = photo.original_size.url,
                        Description = item.summary,
                        Tags = tags.ToArray(),
                        Group = group,
                        Timestamp = item.timestamp,
                        Score = item.note_count
                    };
                    result.Add(resultItem);
                }
            }
            return result.ToArray();
        }

        public string DefaultToken => "XccBbn5kdxb3B7ouiulXNWRyZgNDrUf8wTm252aUHyGNIDX4KY";
        public string DefaultAdditional => "";
        public string ConfigureUrl { get; } = "https://www.tumblr.com/oauth/apps";
        public string ConfigureDescription { get; } = "Пройди по этой ссылке, и нажми там на кнопку \"Зарегистрировать приложение\" и скажи мне \"Ключ клиента OAuth\". Удачи там!";
    }

    public class TumblrSearchResultItem : ISearchResultItem
    {
        public string ItemUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string Engine { get; set; } = "Tumblr";
        public string Group { get; set; }
        public int Score { get; set; }
        public int Timestamp { get; set; }
    }
}
