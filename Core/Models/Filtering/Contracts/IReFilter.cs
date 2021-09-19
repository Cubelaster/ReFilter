using System.Linq;

namespace ReFilter.Core.Models.Filtering.Contracts
{
    public interface IReFilter<T> where T : class, new()
    {
        IQueryable<T> FilterQuery(IQueryable<T> query);
    }
}
