using System;
using ReFilter.ReFilterBuilder;

namespace ReFilter.ReFilterConfigBuilder
{
    internal interface IReSearchConfigBuilder
    {
        Type GetMatchingType<T>() where T : class, new();
        IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new();
    }
}
