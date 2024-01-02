using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ReFilter.Models
{
    public class BasePagedRequest : PagedBase
    {
        /// <summary>
        /// Object meant for mapping into query conditions.
        /// Only requirenment is that property names match destination
        /// </summary>
        public JObject Where { get; set; }

        //public JsonDocument Conditions { get; set; }

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
