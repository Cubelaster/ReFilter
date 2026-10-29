using System.Collections.Generic;
using System.Linq;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;

namespace ReFilter.ReSortBuilder
{
    public interface IReSortBuilder<T> where T : class, new()
    {
        /// <param name="filterRequest">
        /// The request's Where (plus PFC values) deserialized into the filter type matched for T by IReFilterConfigBuilder -
        /// the same object IReFilterBuilder&lt;T&gt;.BuildPredicates gets. Null when the request has no Where/PFC values.
        /// </param>
        IOrderedQueryable<T> BuildSortedQuery(IQueryable<T> query, PropertyFilterConfig propertyFilterConfig, List<PropertyFilterConfig> propertyFilterConfigs, IReFilterRequest filterRequest, bool isFirst = false);
    }
}
