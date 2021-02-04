using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ReFilter.Enums;

namespace ReFilter.Models
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

        public PagedRequest GetPagedRequest(bool returnQueryOnly = false, bool returnResultsOnly = false)
        {
            var pagedRequest = (PagedRequest)MemberwiseClone();
            pagedRequest.ReturnQueryOnly = returnQueryOnly;
            pagedRequest.ReturnResultsOnly = returnResultsOnly;

            return pagedRequest;
        }

        public PagedRequest<T, U> GetPagedRequest<T, U>(Func<List<T>, List<U>> mappingFunction) where T : class, new() where U : class, new()
        {
            var pagedRequest = new PagedRequest<T, U>(this)
            {
                ReturnQueryOnly = false,
                ReturnResultsOnly = true,
                MappingFunction = mappingFunction
            };

            return pagedRequest;
        }
    }
}
