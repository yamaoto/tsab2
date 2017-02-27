using Microsoft.EntityFrameworkCore;
using Rikka.Tsab2.Database.Context.Entities;

namespace Rikka.Tsab2.Database.Context
{
    public class TsabContext: DbContext
    {
        public TsabContext()
        {

        }

        public TsabContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Chat> Chat { get; set; }
        public DbSet<VkWall> VkWall { get; set; }
        public DbSet<VkWallEntry> VkWallEntry { get; set; }
        public DbSet<VkPhoto> VkPhoto { get; set; }
        public DbSet<VkAuth> VkAuth { get; set; }
        public DbSet<SearchEngine> SearchEngine { get; set; }
    }
}
