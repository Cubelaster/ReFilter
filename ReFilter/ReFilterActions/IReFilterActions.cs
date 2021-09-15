using System.Linq;
using System.Threading.Tasks;
using ReFilter.Models;

namespace ReFilter.ReFilterActions
{
    public interface IReFilterActions
    {
        public Task<PagedResult<T>> GetPaged<T>(IQueryable<T> query, PagedRequest pagedRequest) where T : class, new();
        public Task<PagedResult<U>> GetPaged<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new();
        public Task<PagedResult<T>> GetFiltered<T>(IQueryable<T> query, PagedRequest pagedRequest) where T : class, new();
        public Task<PagedResult<U>> GetFiltered<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new();
        public Task<PagedResult<T>> GetBySearchQuery<T>(IQueryable<T> query, BasePagedRequest pagedRequest,
             bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : class, new();
        public Task<PagedResult<U>> GetBySearchQuery<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest,
            bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : class, new() where U : class, new();
        public IQueryable<T> ApplyPagination<T>(IQueryable<T> query, BasePagedRequest pagedRequest) where T : class, new();
        public IQueryable<T> FilterObject<T>(IQueryable<T> query, PagedRequest request) where T : class, new();
    }
}
