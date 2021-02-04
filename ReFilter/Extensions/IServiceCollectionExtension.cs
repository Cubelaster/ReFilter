using System;
using Microsoft.Extensions.DependencyInjection;
using ReFilter.ReFilterActions;
using ReFilter.ReFilterTypeMatcher;

namespace ReFilter.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddReFilter(this IServiceCollection services, Type ReFilterTypeMatcherImplementation)
        {
            services.AddScoped<IReFilterActions, ReFilterActions.ReFilterActions>();
            services.AddScoped(typeof(IReFilterConfigBuilder), ReFilterTypeMatcherImplementation);

            return services;
        }
    }
}
