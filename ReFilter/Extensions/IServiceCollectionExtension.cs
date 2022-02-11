using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReFilter.ReFilterActions;
using ReFilter.ReFilterConfigBuilder;
using ReFilter.ReFilterTypeMatcher;

namespace ReFilter.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddReFilter(this IServiceCollection services,
            Type ReFilterTypeMatcherImplementation,
            Type ReSortTypeMatcherImplementation)
        {
            services.AddScoped<IReFilterActions, ReFilterActions.ReFilterActions>();
            services.AddScoped(typeof(IReFilterConfigBuilder), ReFilterTypeMatcherImplementation);
            services.AddScoped(typeof(IReSortConfigBuilder), ReSortTypeMatcherImplementation);

            return services;
        }

        public static IServiceCollection AddReFilter(this IServiceCollection services,
            Type ReFilterTypeMatcherImplementation,
            Type ReSortTypeMatcherImplementation,
            JsonSerializer Serializer)
        {
            services.AddScoped<IReFilterActions>(x => new ReFilterActions.ReFilterActions(
                x.GetRequiredService<IReFilterConfigBuilder>(),
                x.GetRequiredService<IReSortConfigBuilder>(),
                Serializer));

            services.AddScoped(typeof(IReFilterConfigBuilder), ReFilterTypeMatcherImplementation);
            services.AddScoped(typeof(IReSortConfigBuilder), ReSortTypeMatcherImplementation);

            return services;
        }
    }
}
