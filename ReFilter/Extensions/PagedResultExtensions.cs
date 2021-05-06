using System;
using System.Collections.Generic;
using System.Linq;
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

            newPagedResult.Results = mapFunction(pagedResult.Results ?? new List<T>());

            return newPagedResult;
        }

        public static PagedResult<U> TransformResult<U, T>(this PagedResult<T> pagedResult, Func<IQueryable<T>, List<U>> mapFunction) where T : new() where U : new()
        {
            var newPagedResult = new PagedResult<U>
            {
                PageCount = pagedResult.PageCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                RowCount = pagedResult.RowCount
            };

            newPagedResult.Results = mapFunction(pagedResult.ResultQuery);

            return newPagedResult;
        }
    }
}
