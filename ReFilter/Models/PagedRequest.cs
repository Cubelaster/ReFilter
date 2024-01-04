using System;
using System.Collections.Generic;
using System.Linq;

namespace ReFilter.Models
{
    public class PagedRequest : BasePagedRequest
    {
        public PagedRequest() { }

        public PagedRequest(BasePagedRequest pagedRequest)
        {
            PageIndex = pagedRequest.PageIndex;
            PageSize = pagedRequest.PageSize;
            PropertyFilterConfigs = pagedRequest.PropertyFilterConfigs;
            SearchQuery = pagedRequest.SearchQuery;
            Where = pagedRequest.Where;
            PagedRequests = pagedRequest.PagedRequests;
            PredicateOperator = pagedRequest.PredicateOperator;
            ReturnQuery = false;
            ReturnResults = true;
        }

        public bool ReturnQuery { get; set; } = true;
        public bool ReturnResults { get; set; } = false;
    }

    public class PagedRequest<T, U> : PagedRequest where T : class, new() where U : class, new()
    {
        public PagedRequest(BasePagedRequest pagedRequest) : base(pagedRequest)
        {
            ReturnQuery = true;
            ReturnResults = false;
            MappingFunction = null;
            MappingProjection = null;
        }

        public Func<List<T>, List<U>> MappingFunction { get; set; } = null;
        public Func<IQueryable<T>, List<U>> MappingProjection { get; set; } = null;
    }
}
