using System.Collections.Generic;
using System.Linq;
using ReFilter.Models;

namespace ReFilter.ReSortBuilder
{
    public interface IReSortBuilder<T> where T : class, new()
    {
        IOrderedQueryable<T> BuildSortedQuery(IQueryable<T> query, PropertyFilterConfig propertyFilterConfig, List<PropertyFilterConfig> propertyFilterConfigs, bool isFirst = false);
    }
}
