using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReFilter.Models.Filtering.Contracts
{
    public interface IReFilter<T> where T : class, new()
    {
        /// <summary>
        /// Filters query using AND clause
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryable<T> FilterQuery(IQueryable<T> query);
        /// <summary>
        /// Generates expression which can be applied later on using either clause
        /// </summary>
        /// <returns></returns>
        Expression<Func<T, bool>> GeneratePredicate(IQueryable<T> query = null);
    }
}
