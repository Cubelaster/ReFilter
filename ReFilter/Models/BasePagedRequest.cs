using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Newtonsoft.Json.Linq;

namespace ReFilter.Models
{
    public class BasePagedRequest : PagedBase
    {
        public PredicateOperator PredicateOperator { get; set; } = PredicateOperator.And;

        /// <summary>
        /// Object meant for mapping into query conditions.
        /// Only requirenment is that property names match destination
        /// </summary>
        public JObject Where { get; set; }

        /// <summary>
        /// Defines rules for sorting and filtering
        /// Can be left empty and in such way, the default values are used.
        /// Default values are no sort and Equals comparer
        /// </summary>
        public List<PropertyFilterConfig> PropertyFilterConfigs { get; set; }

        /// <summary>
        /// String SearchQuery meant for searching ANY of the tagged property
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// If you need to filter by multiple incompatible filters, this is the easiest way to do it <para />
        /// Depending on <see cref="PredicateOperator"/> set in parent BasePagedRequest, child requests are added either as AND or OR clauses <para />
        /// Predicate is being built the same way every time so you are able to chain multiple complex filters
        /// </summary>
        public List<PagedRequest> PagedRequests { get; set; }

        public PagedRequest GetPagedRequest(bool returnQuery = true, bool returnResults = false)
        {
            var pagedRequest = new PagedRequest(this)
            {
                ReturnQuery = returnQuery,
                ReturnResults = returnResults
            };

            return pagedRequest;
        }

        public PagedRequest<T, U> GetPagedRequest<T, U>(bool returnQuery = true, bool returnResults = false) where T : class, new() where U : class, new()
        {
            var pagedRequest = new PagedRequest<T, U>(this)
            {
                ReturnQuery = returnQuery,
                ReturnResults = returnResults
            };

            return pagedRequest;
        }

        public PagedRequest<T, U> GetPagedRequest<T, U>(Func<List<T>, List<U>> mappingFunction) where T : class, new() where U : class, new()
        {
            var pagedRequest = new PagedRequest<T, U>(this)
            {
                ReturnQuery = false,
                ReturnResults = true,
                MappingFunction = mappingFunction
            };

            return pagedRequest;
        }

        public PagedRequest<T, U> GetPagedRequest<T, U>(Func<IQueryable<T>, List<U>> mappingProjection) where T : class, new() where U : class, new()
        {
            var pagedRequest = new PagedRequest<T, U>(this)
            {
                ReturnQuery = false,
                ReturnResults = true,
                MappingProjection = mappingProjection
            };

            return pagedRequest;
        }
    }
}
