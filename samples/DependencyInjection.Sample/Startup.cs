using DependencyInjection.Sample.Indexes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Indexes;
using RavenDB.AspNetCore.DependencyInjection;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace DependencyInjection.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRavenManager(
             options =>
             {
                 options.DefaultServer = "Main";
                 options.AddServer("Main", new RavenServerOptions()
                 {
                     Url = "http://localhost:8080",
                     DefaultDatabase = "ravendb-Manager"
                 });
             }).AddScopedAsyncSession();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IRavenManager manager)
        {
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
        }

        protected void ConfigureRaven(IRavenManager manager)
        {
            var store = manager.GetStore();
            IndexCreation.CreateIndexes(typeof(Article_ByName).Assembly, store);
        }
    }
}
