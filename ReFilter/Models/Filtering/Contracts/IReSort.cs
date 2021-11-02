using System.Linq;
using ReFilter.Enums;

namespace ReFilter.Models.Filtering.Contracts
{
    public interface IReSort<T> where T : class, new()
    {
        IOrderedQueryable<T> SortQuery(IQueryable<T> query, SortDirection sortDirection = SortDirection.ASC, bool isFirst = true);
    }
}
