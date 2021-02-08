using System;
using ReFilter.ReFilterProvider;
using ReFilter.ReFilterTypeMatcher;
using TestProject.FilterBuilders;
using TestProject.Models;
using TestProject.Models.FilterRequests;

namespace TestProject.RequiredImplementations
{
    /// <summary>
    /// This one is required because it's used in Startup
    /// Minimum implementation is shown for Student => it's the Default case
    /// Semi Real-Life is shown for School
    /// </summary>
    class ReFilterConfigBuilder : IReFilterConfigBuilder
    {
        public Type GetMatchingType<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                case Type schoolType when schoolType == typeof(School):
                    return typeof(SchoolFilterRequest);
                default:
                    return typeof(T);
            }
        }

        public IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                case Type schoolType when schoolType == typeof(School):
                    return (IReFilterBuilder<T>)new SchoolFilterBuilder();
                default:
                    return null;
            }
        }
    }
}
