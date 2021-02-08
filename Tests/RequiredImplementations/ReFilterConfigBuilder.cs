using System;
using ReFilter.ReFilterProvider;
using ReFilter.ReFilterTypeMatcher;

namespace Tests.RequiredImplementations
{
    /// <summary>
    /// This one is required because it's used in Startup
    /// Minimum implementation is shown
    /// </summary>
    class ReFilterConfigBuilder : IReFilterConfigBuilder
    {
        public Type GetMatchingType<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return typeof(T);
            }
        }

        public IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return null;
            }
        }
    }
}
