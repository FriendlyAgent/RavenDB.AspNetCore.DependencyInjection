using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RavenDB.AspNetCore.DependencyInjection;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace DependencyInjectionWithConfig.Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRaven(
                options =>
                {
                    options.DefaultServer = "Main";
                    options.AddServer("Main", new RavenServerOptions()
                    {
                        Url = "{server url}",
                        DefaultDatabase = "{default database}"
                    });
                    options.AddServer("Logging", new RavenServerOptions()
                    {
                        Url = "{server url}",
                        DefaultDatabase = "{default database}"
                    });
                }).AddAsyncSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}