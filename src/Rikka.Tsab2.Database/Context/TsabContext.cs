using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<VkPhoto>()
                .HasOne<VkWall>(p => p.Wall)
                .WithMany(w => w.Photos)
                .HasForeignKey(f => f.WallId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<Chat> Chat { get; set; }
        public DbSet<VkWall> VkWall { get; set; }
        public DbSet<VkWallEntry> VkWallEntry { get; set; }
        public DbSet<VkPhoto> VkPhoto { get; set; }
        public DbSet<VkAuth> VkAuth { get; set; }
        public DbSet<SearchEngine> SearchEngine { get; set; }
    }
}
