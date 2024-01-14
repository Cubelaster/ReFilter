using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Models.Filtering.Contracts;

namespace ReFilter.ReFilterProvider
{
    public interface IReFilterBuilder<T> where T : class, new()
    {
        /// <summary>
        /// Gets all the custom filters and matches them to properties.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IEnumerable<IReFilter<T>> GetFilters(IReFilterRequest filterRequest);
        /// <summary>
        /// Entry point of filter builder. Builds default query for that entity.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IQueryable<T> BuildEntityQuery(IReFilterRequest filterRequest);
        /// <summary>
        /// First uses GetFilters and then applies them to the provided query <para />
        /// This builds query using AND clauses <para />
        /// It is essentially not used anymore and is replaced by BuildPredicates
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IQueryable<T> BuildFilteredQuery(IQueryable<T> query, IReFilterRequest filterRequest);
        /// <summary>
        /// Builds predicates one by one
        /// Predicates can later on be used as And/Or clauses
        /// This is the intended way and is used under the hood to build and apply filters
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        List<Expression<Func<T, bool>>> BuildPredicates(IReFilterRequest filterRequest, IQueryable<T> query = null);
        /// <summary>
        /// Gets the list of Ids for the provided filter parameters in order to use it as an "IN ()" clause.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        List<int> GetForeignKeys(IReFilterRequest filterRequest);
    }
}
