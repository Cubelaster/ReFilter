using System;
using System.Collections.Generic;

namespace ReFilter.Models
{
    public class PagedRequest : BasePagedRequest
    {
        public bool ReturnQueryOnly { get; set; } = false;
        public bool ReturnResultsOnly { get; set; } = false;
    }

    public class PagedRequest<T, U> : PagedRequest where T : class, new() where U : class, new()
    {
        public PagedRequest(BasePagedRequest pagedRequest)
        {
            PageIndex = pagedRequest.PageIndex;
            PageSize = pagedRequest.PageSize;
            PropertyFilterConfigs = pagedRequest.PropertyFilterConfigs;
            SearchQuery = SearchQuery;
            Sorting = pagedRequest.Sorting;
            Where = pagedRequest.Where;
            ReturnQueryOnly = false;
            ReturnResultsOnly = true;
            MappingFunction = null;
        }

        public Func<List<T>, List<U>> MappingFunction { get; set; } = null;
    }
}
