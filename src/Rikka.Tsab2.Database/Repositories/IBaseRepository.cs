using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rikka.Tsab2.Database.Context;
using Rikka.Tsab2.Database.Context.Entities;

namespace Rikka.Tsab2.Database.Repositories
{

    public class BaseRepository<T> where T : class,IEntity
    {
        protected readonly TsabContext Context;
        protected readonly DbSet<T> DbSet;

        public BaseRepository(TsabContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public async Task<T> GetById(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task Insert(T item)
        {
            if (item.Id==Guid.Empty)
                item.Id=Guid.NewGuid();
            item.CreatedOn = DateTime.Now;
            await DbSet.AddAsync(item);
            await Context.SaveChangesAsync();
        }

        public async Task Update(T item)
        {
            DbSet.Update(item);
            await Context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var item = await GetById(id);
            await Delete(item);
            await Context.SaveChangesAsync();
        }

        public async Task Delete(T item)
        {
            DbSet.Remove(item);
            await Context.SaveChangesAsync();
        }

    }

    public interface IChatRepository
    {
        Task<Chat> GetById(Guid id);
        Task Insert(Chat item);
        Task Update(Chat item);
        Task Delete(Guid id);
        Task Delete(Chat item);

        Task<Chat> GetyChatId(int chatId);
        Task SetState(int chatId, string state, string stateParams = null);
        Task<string> GetStateParams(int chatId);
    }

    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(TsabContext context) : base(context)
        {
        }

        public async Task<Chat> GetyChatId(int chatId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.ChatId == chatId);
        }

        public async Task SetState(int chatId, string state, string stateParams = null)
        {
            var item = await this.GetyChatId(chatId);
            if (item != null)
            {
                item.State = state;
                item.StateParams = stateParams;
            }
            await Context.SaveChangesAsync();
        }

        public async Task<string> GetStateParams(int chatId)
        {
            var item = await  DbSet.FirstOrDefaultAsync(f => f.ChatId == chatId);
            return item?.StateParams;
        }
    }

    public interface IVkWallRepository
    {
        Task<VkWall> GetById(Guid id);
        Task Insert(VkWall item);
        Task Update(VkWall item);
        Task Delete(Guid id);
        Task Delete(VkWall item);
    }

    public class VkWallRepository : BaseRepository<VkWall>, IVkWallRepository
    {
        public VkWallRepository(TsabContext context) : base(context)
        {
        }
    }

    public interface IVkWallEntryRepository
    {
        Task<VkWallEntry> GetById(Guid id);
        Task Insert(VkWallEntry item);
        Task Update(VkWallEntry item);
        Task Delete(Guid id);
        Task Delete(VkWallEntry item);
    }

    public class VkWallEntryRepository : BaseRepository<VkWallEntry>, IVkWallEntryRepository
    {
        public VkWallEntryRepository(TsabContext context) : base(context)
        {
        }
    }

    public interface IVkPhotoRepository
    {
        Task<VkPhoto> GetById(Guid id);
        Task Insert(VkPhoto item);
        Task Update(VkPhoto item);
        Task Delete(Guid id);
        Task Delete(VkPhoto item);
    }

    public class VkPhotoRepository : BaseRepository<VkPhoto>, IVkPhotoRepository
    {
        public VkPhotoRepository(TsabContext context) : base(context)
        {
        }
    }

    public interface IVkAuthRepository
    {
        Task<VkAuth> GetById(Guid id);
        Task Insert(VkAuth item);
        Task Update(VkAuth item);
        Task Delete(Guid id);
        Task Delete(VkAuth item);
    }

    public class VkAuthRepository : BaseRepository<VkAuth>, IVkAuthRepository
    {
        public VkAuthRepository(TsabContext context) : base(context)
        {
        }
    }

    public interface ISearchEngineRepository
    {
        Task<SearchEngine> GetById(Guid id);
        Task Insert(SearchEngine item);
        Task Update(SearchEngine item);
        Task Delete(Guid id);
        Task Delete(SearchEngine item);
        Task<SearchEngine> GetEngine(string name, int chatId);
        Task<IEnumerable<SearchEngine>>  GetEngines(int chatId);
    }

    public class SearchEngineRepository : BaseRepository<SearchEngine>, ISearchEngineRepository
    {
        public SearchEngineRepository(TsabContext context) : base(context)
        {
        }

        public async Task<SearchEngine> GetEngine(string name, int chatId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Name == name && f.ChatId == chatId);
        }

        public async Task<IEnumerable<SearchEngine>> GetEngines(int chatId)
        {
            return await DbSet.Where(w => w.ChatId == chatId).ToArrayAsync();
        }
    }

    public interface ISearchHistoryRepository
    {
        Task<SearchHistory> GetById(Guid id);
        Task Insert(SearchHistory item);
        Task Update(SearchHistory item);
        Task Delete(Guid id);
        Task Delete(SearchHistory item);
        Task<Dictionary<string,int>>  GetPopularTags(int chatId, int limit=5);
    }

    public class SearchHistoryRepository : BaseRepository<SearchHistory>, ISearchHistoryRepository
    {
        public SearchHistoryRepository(TsabContext context) : base(context)
        {
        }

        public async Task<Dictionary<string, int>> GetPopularTags(int chatId, int limit = 5)
        {
            var items = DbSet.Where(w => w.ChatId == chatId);
            var tags =
                await items.GroupBy(g => g.Expression)
                    .Select(g => new {g.Key, Count = g.Count()})
                    .OrderByDescending(o => o.Count)
                    .Take(limit)
                    .ToArrayAsync();
            return tags.ToDictionary(k => k.Key, v => v.Count);
        }
    }

    public interface IDataAnalyticsRepository
    {
        Task<DataAnalytics> GetByData(string data);
        Task<DataAnalytics> GetById(Guid id);
        Task Insert(DataAnalytics item);
        Task Update(DataAnalytics item);
        Task Delete(Guid id);
        Task Delete(DataAnalytics item);
        Task<IEnumerable<DataAnalytics>> GetByType(string type);
    }

    public class DataAnalyticsRepository : BaseRepository<DataAnalytics>, IDataAnalyticsRepository
    {
        public DataAnalyticsRepository(TsabContext context) : base(context)
        {
        }

        public async Task<DataAnalytics> GetByData(string data)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Data==data);
        }

        public async Task<IEnumerable<DataAnalytics>> GetByType(string type)
        {
            var items = DbSet.Where(w => w.Type == type);
            return await items.ToArrayAsync();
        }
    }
}
