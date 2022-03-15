using System.Linq;
using ReFilter.Models.Filtering.Contracts;

namespace ReFilter.ReSearchBuilder
{
    internal interface IReSearchBuilder<T> where T : class, new()
    {
        IQueryable<T> BuildSearchQuery(IReFilterRequest filterRequest);
    }
}
