using System;
using DependencyInjection.Sample.Indexes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Indexes;
using RavenDB.AspNetCore.DependencyInjection;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace DependencyInjection.Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {                        
            // Use configuration from appsettings.json
            services.AddRavenManager(Configuration.GetSection("Raven")).AddScopedAsyncSession();

            // Use configuration from appsettings.json
            //services.AddRavenManagerWithDefaultServer(options => {
            //    options.Url = "{server url}";
            //    options.Database = "{database name}";
            //}).AddScopedAsyncSession();


            // Configure full options
            // Add framework services.
            //services.AddRavenManager(
            //  options =>
            //  {
            //      options.DefaultServer = "Main";
            //      options.AddServer("Main", new RavenServerOptions()
            //      {
            //          Url = "{server url}",
            //          Database = "{database name}"
            //      });
            //      options.AddServer("Logging", new RavenServerOptions()
            //      {
            //          Url = "{server url}",
            //         Database = "{database name}"
            //      });
            //  }).AddScopedAsyncSession();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IRavenManager ravenManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ConfigureRaven(ravenManager);
        }

        protected void ConfigureRaven(IRavenManager manager) {
            var store = manager.GetStore();

            IndexCreation.CreateIndexes(typeof(Tests_ByName).Assembly, store);
        }
    }
}
