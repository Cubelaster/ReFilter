using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;

namespace ReFilter.ReFilterBuilder
{
    public interface IReFilterBuilder<T> where T : class, new()
    {
        /// <summary>
        /// Builds predicates one by one
        /// Predicates can later on be used as And/Or clauses
        /// This is the intended way and is used under the hood to build and apply filters
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        List<Expression<Func<T, bool>>> BuildPredicates(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs, IQueryable<T> query = null);
    }
}
