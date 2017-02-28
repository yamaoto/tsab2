using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Rikka.Tsab2.Database.Context;

namespace Rikka.Tsab2.Database.Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {

        }
    }

    public class TemporaryDbContextFactory : IDbContextFactory<TsabContext>
    {
        public TsabContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<TsabContext>();
            builder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=tsab2;Trusted_Connection=True;",
                 b => b.MigrationsAssembly("Rikka.Tsab2.Database.Generator"));
            return new TsabContext(builder.Options);
        }
    }
}
