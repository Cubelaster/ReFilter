using System.Linq;

namespace ReFilter.Models.Filtering.Contracts
{
    public interface IReSort<T> where T : class, new()
    {
        IOrderedQueryable<T> SortQuery(IQueryable<T> query, bool isFirst = true);
    }
}
