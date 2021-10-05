using System;
using ReFilter.ReSortBuilder;

namespace ReFilter.ReFilterConfigBuilder
{
    public interface IReSortConfigBuilder
    {
        Type GetMatchingType<T>() where T : class, new();
        IReSortBuilder<T> GetMatchingSortBuilder<T>() where T : class, new();
    }
}
