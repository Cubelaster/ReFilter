using System.Collections.Generic;
using System.Linq;
using ReFilter.Core.Models.Filtering.Contracts;

namespace ReFilter.ReFilterProvider
{
    public interface IReFilterBuilder<T> where T : class, new()
    {
        IEnumerable<IReFilter<T>> GetFilters(IReFilterRequest filterRequest);
        IQueryable<T> BuildEntityQuery(IReFilterRequest filterRequest);
        IQueryable<T> BuildFilteredQuery(IQueryable<T> query, IReFilterRequest filterRequest);
        List<int> GetForeignKeys(IReFilterRequest filterRequest);
    }
}
