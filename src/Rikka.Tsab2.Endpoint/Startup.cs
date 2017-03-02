using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Rikka.TelegramBotCore;
using Rikka.Tsab2.Core.Services;
using Rikka.Tsab2.Database.Context;
using Rikka.Tsab2.Database.Repositories;
using Rikka.Tsab2.Endpoint.App.Filters;
using System.IO;
using Rikka.Tsab2.Core.Workers;

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
                .AddJsonFile("password.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services,ILoggerFactory loggerFactory)
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
            services.AddTransient<ExceptionFilter>();
            services.AddSingleton<IWorkerService>();
            #endregion

            #region bot
            services.AddTransient<IBotActionsService, BotActionsService>();
            var botApi = new BotApi(Configuration["Telegram:Token"]);
            services.AddSingleton<IBotApi>(botApi);
            services.AddTransient<IBotService, BotService>();
            #endregion bot

            //services.AddRouting();
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            services.AddSingleton(_ => Configuration);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IWorkerService workerService)
        {            
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            //if (!Directory.Exists("Logs"))
            //    Directory.CreateDirectory("Logs");
            //loggerFactory.AddFile("Logs/tsab2-{Date}.txt");

            app.UseStaticFiles();
            app.UseMvc();

            workerService.RegisterWorker<SearchWorker,SearchWorkerParameter>("search");
        }
    }
}
