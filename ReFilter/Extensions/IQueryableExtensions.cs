using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ReFilter.Attributes;
using ReFilter.Enums;
using ReFilter.Models;
using ReFilter.Utilities;

namespace ReFilter.Extensions
{
    public static class IQueryableExtensions
    {
        #region Pagination

        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, PagedRequest pagedRequest) where T : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                if (pagedRequest.PropertyFilterConfigs != null && pagedRequest.PropertyFilterConfigs.Any())
                {
                    query = query.FilterObject(pagedRequest);
                }

                var resultQuery = ApplyPagination<T>(query, pagedRequest);

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnQueryOnly ? new List<T>() : await Task.FromResult(resultQuery.ToList());
                result.ResultQuery = pagedRequest.ReturnResultsOnly ? null : resultQuery;
                return result;
            }

            return new PagedResult<T>
            {
                Results = new List<T>(),
                ResultQuery = query
            };
        }

        public static async Task<PagedResult<U>> GetPaged<T, U>(this IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                if (pagedRequest.PropertyFilterConfigs != null && pagedRequest.PropertyFilterConfigs.Any())
                {
                    query = query.FilterObject(pagedRequest);
                }

                var resultQuery = ApplyPagination(query, pagedRequest);

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnQueryOnly ? new List<T>() : await Task.FromResult(resultQuery.ToList());
                result.ResultQuery = pagedRequest.ReturnResultsOnly ? null : resultQuery;
                return result.TransformResult(pagedRequest.MappingFunction);
            }

            return new PagedResult<U>
            {
                Results = new List<U>(),
                ResultQuery = null
            };
        }

        #endregion Pagination

        #region Filtering

        public static async Task<PagedResult<T>> GetFiltered<T>(this IQueryable<T> query, PagedRequest pagedRequest) where T : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                if (pagedRequest.PropertyFilterConfigs != null && pagedRequest.PropertyFilterConfigs.Any())
                {
                    query = query.FilterObject(pagedRequest);
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnQueryOnly ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = pagedRequest.ReturnResultsOnly ? null : query;
                return result;
            }

            return new PagedResult<T>
            {
                Results = new List<T>(),
                ResultQuery = query
            };
        }

        public static async Task<PagedResult<U>> GetFiltered<T, U>(this IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                if (pagedRequest.PropertyFilterConfigs != null && pagedRequest.PropertyFilterConfigs.Any())
                {
                    query = query.FilterObject(pagedRequest);
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnQueryOnly ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = pagedRequest.ReturnResultsOnly ? null : query;
                return result.TransformResult(pagedRequest.MappingFunction);
            }

            return new PagedResult<U>
            {
                Results = new List<U>(),
                ResultQuery = null
            };
        }

        #endregion Filtering

        #region SearchQueries

        public static async Task<PagedResult<T>> GetBySearchQuery<T>(this IQueryable<T> query, BasePagedRequest pagedRequest,
            bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();
            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = applyPagination ? pagedRequest.PageIndex : default,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                List<PropertyInfo> searchableProperties;
                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    searchableProperties = objectType
                        .GetProperties()
                        .Where(p => p.GetCustomAttributes().OfType<ReFilterProperty>().Any(e => e.UsedForSearchQuery))
                        .ToList();

                    if (searchableProperties.Any())
                    {
                        query = query
                        .Where(q => searchableProperties
                            .Any(p => p.GetValue(q).ToString()
                            .Contains(pagedRequest.SearchQuery, StringComparison.OrdinalIgnoreCase)));
                    }
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                if (applyPagination)
                {
                    query = ApplyPagination<T>(query, pagedRequest);
                }

                result.Results = returnQueryOnly ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = returnResultsOnly ? null : query;
                return result;
            }

            return new PagedResult<T>
            {
                Results = new List<T>()
            };
        }

        public static async Task<PagedResult<U>> GetBySearchQuery<T, U>(this IQueryable<T> query, BasePagedRequest pagedRequest,
            Func<List<T>, List<U>> mappingFunction,
            bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();
            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = applyPagination ? pagedRequest.PageIndex : default,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.Sorting != null && pagedRequest.Sorting.Keys.Count > 0)
                {
                    query = SortObject(pagedRequest.Sorting, query);
                }

                List<PropertyInfo> searchableProperties;
                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    searchableProperties = objectType
                        .GetProperties()
                        .Where(p => p.GetCustomAttributes().OfType<ReFilterProperty>().Any(e => e.UsedForSearchQuery))
                        .ToList();

                    if (searchableProperties.Any())
                    {
                        query = query
                        .Where(q => searchableProperties
                            .Any(p => p.GetValue(q).ToString()
                            .Contains(pagedRequest.SearchQuery, StringComparison.OrdinalIgnoreCase)));
                    }
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                if (applyPagination)
                {
                    query = ApplyPagination<T>(query, pagedRequest);
                }

                result.Results = returnQueryOnly ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = returnResultsOnly ? null : query;
                return result.TransformResult(mappingFunction);
            }

            return new PagedResult<U>
            {
                Results = new List<U>()
            };
        }

        #endregion SearchQueries

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, BasePagedRequest pagedRequest)
        {
            int skip = pagedRequest.PageIndex * pagedRequest.PageSize;
            return query.Skip(skip).Take(pagedRequest.PageSize);
        }

        #region Sorts

        private static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source,
            string propertyName, string methodName)
        {
            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(TSource), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == methodName && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TSource), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<TSource>)result;
        }

        private static IOrderedQueryable<TSource> ThenOrderBy<TSource>(this IOrderedQueryable<TSource> source,
            string propertyName, string methodName)
        {
            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(TSource), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == methodName && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(TSource), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<TSource>)result;
        }

        private static IQueryable<T> SortObject<T>(Dictionary<string, SortDirection> sortPairs, IQueryable<T> query) where T : new()
        {
            var sorts = sortPairs.ToList();

            if (sorts.Count > 0)
            {
                var firstSort = sorts.First();

                IOrderedQueryable<T> orderedQuery;

                var methodName = firstSort.Value.GetOrderByNames();
                orderedQuery = query.OrderBy(firstSort.Key, methodName);

                foreach (var sort in sorts.Skip(1))
                {
                    methodName = sort.Value.GetOrderByNames(true);
                    orderedQuery = orderedQuery.ThenOrderBy(firstSort.Key, methodName);
                }

                return orderedQuery;
            }

            return query;
        }

        #endregion Sorts

        public static IQueryable<T> FilterObject<T>(this IQueryable<T> query, PagedRequest request) where T : class, new()
        {
            if (request.PropertyFilterConfigs.Any())
            {
                var filterObjectType = FilterHelper.GetMatchingType<T>();
                var filterObject = request.Where.ToObject(filterObjectType);

                var filterValues = filterObject.GetObjectPropertiesWithValue();

                if (filterValues.Keys.Any())
                {
                    filterValues.Keys.ToList().ForEach(fv =>
                    {
                        var selectedPfc = request.PropertyFilterConfigs?.FirstOrDefault(pfc => pfc.PropertyName == fv)
                            ?? new PropertyFilterConfig
                            {
                                PropertyName = fv
                            };
                        selectedPfc.Value = filterValues[fv];

                        var predicate = ReFilterExpressionBuilder.ReFilterExpressionBuilder.BuildPredicate<T>(selectedPfc);
                        query = query.Where(predicate);
                    });
                }
            }

            return query;
        }
    }
}
