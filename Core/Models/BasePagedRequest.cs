using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ReFilter.Core.Enums;

namespace ReFilter.Core.Models
{
    public class BasePagedRequest : PagedBase
    {
        /// <summary>
        /// Where object for 1:1 mapping to entity to be filtered.
        /// Only requirenment is that property names are same
        /// </summary>
        public JObject Where { get; set; }

        /// <summary>
        /// Defines rules for sorting and filtering
        /// Can be left empty and in such way, the default values are used.
        /// Default values are no sort and Equals comparer
        /// </summary>
        public List<PropertyFilterConfig> PropertyFilterConfigs { get; set; }

        [Obsolete]
        /// <summary>
        /// Dictionary containing Keys matching PropertyNames and Value matching SortDirection
        /// </summary>
        public Dictionary<string, SortDirection> Sorting { get; set; }

        /// <summary>
        /// String SearchQuery meant for searching ANY of the tagged property
        /// </summary>
        public string SearchQuery { get; set; }

        public PagedRequest GetPagedRequest(bool returnQuery = true, bool returnResults = false)
        {
            var pagedRequest = new PagedRequest
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                PropertyFilterConfigs = PropertyFilterConfigs,
                SearchQuery = SearchQuery,
                Sorting = Sorting,
                Where = Where,
                ReturnQuery = returnQuery,
                ReturnResults = returnResults
            };

            return pagedRequest;
        }

        public PagedRequest<T, U> GetPagedRequest<T, U>(bool returnQuery = true, bool returnResults = false) where T : class, new() where U : class, new()
        {
            var pagedRequest = new PagedRequest<T, U>(this)
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                PropertyFilterConfigs = PropertyFilterConfigs,
                SearchQuery = SearchQuery,
                Sorting = Sorting,
                Where = Where,
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
