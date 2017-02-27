using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rikka.TelegramBotCore;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Context;
using Rikka.Tsab2.Database.Repositories;

namespace Rikka.Tsab2.Endpoint
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("database.password.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region database
            var connection = Configuration["Database:ConnectionStrings:DefaultDatabase"];
            var password = Configuration["Database:Passwords:DefaultDatabase"];
            connection = string.Format(connection, password);
            services.AddDbContext<TsabContext>(options => options.UseSqlServer(connection));
            #endregion

            #region repositories
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<IVkWallRepository, VkWallRepository>();
            services.AddTransient<IVkWallEntryRepository, VkWallEntryRepository>();
            services.AddTransient<IVkPhotoRepository, VkPhotoRepository>();
            services.AddTransient<IVkAuthRepository, VkAuthRepository>();
            services.AddTransient<ISearchEngineRepository, SearchEngineRepository>();
            #endregion

            #region services
            services.AddTransient<ISearchService, SearchService>();
            #endregion

            #region bot
            services.AddTransient<IBotActionsService, BotActionsService>();
            var botApi = new BotApi(Configuration["Telegram:Token"]);
            services.AddSingleton<IBotApi>(botApi);
            services.AddTransient<IBotService, BotService>();
            #endregion bot

            services.AddMvc();
            services.AddSingleton(_ => Configuration);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
        }
    }
}
