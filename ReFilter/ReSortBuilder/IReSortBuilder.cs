using System.Collections.Generic;
using System.Linq;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;

namespace ReFilter.ReSortBuilder
{
    public interface IReSortBuilder<T> where T : class, new()
    {
        List<IReSort<T>> GetSorters(PropertyFilterConfig propertyFilterConfig);
        IQueryable<T> BuildEntityQuery();
        IOrderedQueryable<T> BuildSortedQuery(IQueryable<T> query, PropertyFilterConfig propertyFilterConfig, bool isFirst = false);
    }
}
