using System;
using System.Collections.Generic;
using ReFilter.Models;

namespace ReFilter.Extensions
{
    public static class PagedResultExtensions
    {
        public static PagedResult<U> TransformResult<U, T>(this PagedResult<T> pagedResult, Func<List<T>, List<U>> mapFunction) where T : new() where U : new()
        {
            var newPagedResult = new PagedResult<U>
            {
                PageCount = pagedResult.PageCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                RowCount = pagedResult.RowCount
            };

            newPagedResult.Results = mapFunction(pagedResult.Results);

            return newPagedResult;
        }
    }
}
