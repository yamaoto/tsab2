using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Rikka.Tsab2.Core.Services.Analytics;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Core.Services
{
    public class AnswerAnalyticService
    {
        private readonly IDataAnalyticsService _dataAnalyticsService;
        private readonly IAnalyticFetcherService _analyticFetcherService;

        public AnswerAnalyticService(IDataAnalyticsService dataAnalyticsService, IAnalyticFetcherService analyticFetcherService)
        {
            _dataAnalyticsService = dataAnalyticsService;
            _analyticFetcherService = analyticFetcherService;
        }

        public async Task<bool> GetBool(string answer)
        {
            var items = await _dataAnalyticsService.GetAnalytics(type => type.Answer);
            var analytics = _analyticFetcherService.Fetch(items);
            var emotional = analytics.Counters.OfType<EmotionalCounter>();
            double positive;
            double negative;
            foreach (var counter in emotional)
            {

            }
            throw new NotImplementedException();

        }
    }


   
}