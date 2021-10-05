using System;
using ReFilter.ReFilterConfigBuilder;
using ReFilter.ReSortBuilder;
using TestProject.Models;
using TestProject.Models.FilterRequests;

namespace TestProject.RequiredImplementations
{
    internal class ReSortConfigBuilder : IReSortConfigBuilder
    {
        public IReSortBuilder<T> GetMatchingSortBuilder<T>() where T : class, new()
        {
            return null;
        }

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
    }
}
