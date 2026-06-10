using System;
using ReFilter.ReFilterBuilder;

namespace ReFilter.ReFilterTypeMatcher
{
    public interface IReFilterConfigBuilder
    {
        Type GetMatchingType<T>() where T : class, new();
        IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new();
    }
}
