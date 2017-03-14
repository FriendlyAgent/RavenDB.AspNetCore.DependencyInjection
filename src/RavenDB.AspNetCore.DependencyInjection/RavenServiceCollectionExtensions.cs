using Microsoft.Extensions.DependencyInjection;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public static class RavenServiceCollectionExtensions
    {
        public static RavenBuilder AddRaven(
            this IServiceCollection services)
        {
            return AddRaven(services, null);
        }

        public static RavenBuilder AddRaven(
            this IServiceCollection services,
            Action<RavenOptions> options)
        {
            return AddRaven<RavenManager, RavenOptions>(services, options);
        }

        public static RavenBuilder AddRaven<TValue, TOptions>(
               this IServiceCollection services,
               Action<TOptions> options = null)
                where TOptions : class
                where TValue : class, IRavenManager
        {
            if (options != null)
                services.Configure(options);

            return AddRaven<TValue>(services);
        }

        public static RavenBuilder AddRaven<TValue>(
            this IServiceCollection services)
            where TValue : class, IRavenManager
        {
            services.AddScoped<IRavenManager, TValue>();

            return new RavenBuilder(services);
        }
    }
}