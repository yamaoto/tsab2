using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rikka.Tsab2.Core.Services.Analytics;
using Rikka.Tsab2.Database.Context.Entities;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.Services
{
    public interface IDataAnalyticsService
    {
        BaseAnalytics Deserialize(string json);
        string Serialize(BaseAnalytics analytics);
        Task StoreData(string data,Expression<Func<DataAnalyticsType,string>> typeExpression,string dataInformation =null,string context=null);
        Task AppendAnalytics(string data, Expression<Func<DataAnalyticsType, string>> typeExpression, string dataInformation,IEnumerable<ICounter> counters);
        Task<IEnumerable<BaseAnalytics>> GetAnalytics(Expression<Func<DataAnalyticsType, string>> typeExpression);
    }

    public class DataAnalyticsService : IDataAnalyticsService
    {
        private readonly IDataAnalyticsRepository _dataAnalyticsRepository;

        public DataAnalyticsService(IDataAnalyticsRepository dataAnalyticsRepository)
        {
            _dataAnalyticsRepository = dataAnalyticsRepository;
        }

        public BaseAnalytics Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<BaseAnalytics>(json);
        }

        public string Serialize(BaseAnalytics analytics)
        {
            return JsonConvert.SerializeObject(analytics);
        }

        public async Task StoreData(string data,Expression<Func<DataAnalyticsType,string>> typeExpression,string dataInformation =null,string context=null)
        {
            var counters = new List<ICounter> {new HitCounter(DateTime.Now)};
            if(!string.IsNullOrEmpty(context))
                counters.Add(new ContextCounter(context,1));
            await AppendAnalytics(data, typeExpression, dataInformation, counters);            
        }

        public async Task AppendAnalytics(string data, Expression<Func<DataAnalyticsType, string>> typeExpression, string dataInformation,IEnumerable<ICounter> counters)
        {
            var typeBody = typeExpression.Compile();
            var type = typeBody.Invoke(new DataAnalyticsType());
            var item = await _dataAnalyticsRepository.GetByData(data);
            if (item == null)
            {
                item = new DataAnalytics(data, type, dataInformation, Serialize(new BaseAnalytics()));
                await _dataAnalyticsRepository.Insert(item);
            }
            item.DataInformation = dataInformation;
            var analytics = Deserialize(item.Analytics);

            foreach (var counter in counters)
            {
                if (counter is HitCounter || counter is EmotionalCounter)
                {
                    analytics.Counters.Add(counter);
                }
                else
                {
                    if (!analytics.Counters.Contains(counter))
                    {
                        analytics.Counters.Add(counter);
                    }
                }
            }
            
            item.Analytics = Serialize(analytics);
            await _dataAnalyticsRepository.Update(item);
        }

        public async Task<IEnumerable<BaseAnalytics>> GetAnalytics(Expression<Func<DataAnalyticsType, string>> typeExpression)
        {
            var typeBody = typeExpression.Compile();
            var type = typeBody.Invoke(new DataAnalyticsType());
            var items = await _dataAnalyticsRepository.GetByType(type);
            return items.Select(s => Deserialize(s.Analytics));
        }
    }

    public class DataAnalyticsType
    {
        public string Sticker => "sticker";
        public string Text => "text";
        public string Answer => "answer";
    }
}
