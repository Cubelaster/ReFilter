using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LinqKit;
using ReFilter.Enums;
using ReFilter.Extensions;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterConfigBuilder;
using ReFilter.ReFilterTypeMatcher;

namespace ReFilter.ReFilterActions
{
    public class ReFilterActions : IReFilterActions
    {
        #region Ctors and Members

        private readonly IReFilterConfigBuilder reFilterTypeMatcher;
        private readonly IReSortConfigBuilder reSortConfigBuilder;

        public ReFilterActions(IReFilterConfigBuilder reFilterTypeMatcher, IReSortConfigBuilder reSortConfigBuilder)
        {
            this.reFilterTypeMatcher = reFilterTypeMatcher;
            this.reSortConfigBuilder = reSortConfigBuilder;
        }

        #endregion Ctors and Members

        #region Pagination

        /// <summary>
        /// This is a basic GetPaged
        /// It has options which enable you to select either the query or the list return types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagedRequest"></param>
        /// <returns></returns>
        public async Task<PagedResult<T>> GetPaged<T>(IQueryable<T> query, PagedRequest pagedRequest) where T : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (pagedRequest.Where != null)
                {
                    query = FilterObject(query, pagedRequest);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
                }

                var resultQuery = ApplyPagination<T>(query, pagedRequest);

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnResults ? await Task.FromResult(resultQuery.ToList()) : new List<T>();
                result.ResultQuery = pagedRequest.ReturnQuery ? resultQuery : null;
                return result;
            }

            return new PagedResult<T>
            {
                Results = new List<T>(),
                ResultQuery = query
            };
        }

        /// <summary>
        /// Advanced option of GetPaged
        /// It automatically returns mapped result and it does not return the query, since it is already resolved
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagedRequest"></param>
        /// <returns></returns>
        public async Task<PagedResult<U>> GetPaged<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (pagedRequest.Where != null)
                {
                    query = FilterObject(query, pagedRequest);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
                }

                var resultQuery = ApplyPagination(query, pagedRequest);

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                return await Task.FromResult(result.TransformResult(pagedRequest, resultQuery));
            }

            return new PagedResult<U>()
            {
                Results = new List<U>()
            };
        }

        public IQueryable<T> ApplyPagination<T>(IQueryable<T> query, BasePagedRequest pagedRequest) where T : class, new()
        {
            int skip = pagedRequest.PageIndex * pagedRequest.PageSize;
            return query.Skip(skip).Take(pagedRequest.PageSize);
        }

        #endregion Pagination

        #region Filtering

        public async Task<PagedResult<T>> GetFiltered<T>(IQueryable<T> query, PagedRequest pagedRequest) where T : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (pagedRequest.Where != null)
                {
                    query = FilterObject(query, pagedRequest);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                result.Results = pagedRequest.ReturnResults ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = pagedRequest.ReturnQuery ? null : query;
                return result;
            }

            return new PagedResult<T>
            {
                Results = new List<T>(),
                ResultQuery = query
            };
        }

        public async Task<PagedResult<U>> GetFiltered<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest) where T : class, new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();

            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = pagedRequest.PageIndex,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (pagedRequest.Where != null)
                {
                    query = FilterObject(query, pagedRequest);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                return await Task.FromResult(result.TransformResult(pagedRequest, query));
            }

            return new PagedResult<U>
            {
                Results = new List<U>(),
                ResultQuery = null
            };
        }

        public IQueryable<T> FilterObject<T>(IQueryable<T> query, PagedRequest request) where T : class, new()
        {
            var filterObjectType = reFilterTypeMatcher.GetMatchingType<T>();
            var filterObject = request.Where.ToObject(filterObjectType);

            var filterValues = filterObject.GetObjectPropertiesWithValue();
            var specialFilterProperties = filterObjectType.GetSpecialFilterProperties();

            if (filterValues.Keys.Any())
            {
                filterValues.Keys.Where(fk => !specialFilterProperties.Any(sfp => sfp.Name == fk)).ToList().ForEach(fv =>
                {
                    var filterValue = filterValues[fv];
                    if (filterValue.GetType().Name == typeof(RangeFilter<>).Name)
                    {
                        var selectedPfc = request.PropertyFilterConfigs?.FirstOrDefault(pfc => pfc.PropertyName == fv);

                        Type type = filterValue.GetType().GetGenericArguments()[0];
                        var methodInfo = GetType().GetMethod(nameof(UnpackRangeFilter))
                             .MakeGenericMethod(type);
                        List<PropertyFilterConfig> newPropertyFilterConfigs = (List<PropertyFilterConfig>)
                            methodInfo.Invoke(this, new object[] { filterValue, selectedPfc });

                        newPropertyFilterConfigs.ForEach(npfc =>
                        {
                            var predicate = new ReFilterExpressionBuilder.ReFilterExpressionBuilder().BuildPredicate<T>(npfc);
                            query = query.Where(predicate);
                        });
                    }
                    else if (filterValue.GetType() is IReFilterRequest)
                    {

                    }
                    else
                    {
                        var selectedPfc = request.PropertyFilterConfigs?.FirstOrDefault(pfc => pfc.PropertyName == fv)
                        ?? new PropertyFilterConfig
                        {
                            PropertyName = fv
                        };
                        selectedPfc.Value = filterValues[fv];
                        var predicate = new ReFilterExpressionBuilder.ReFilterExpressionBuilder().BuildPredicate<T>(selectedPfc);
                        query = query.Where(predicate);
                    }
                });

                if (filterValues.Keys.Any(fk => specialFilterProperties.Any(sfp => sfp.Name == fk)))
                {
                    var filterBuilder = reFilterTypeMatcher.GetMatchingFilterBuilder<T>();
                    query = filterBuilder.BuildFilteredQuery(query, filterObject as IReFilterRequest);
                }
            }

            return query;
        }

        public List<PropertyFilterConfig> UnpackRangeFilter<U>(RangeFilter<U> rangeFilter, PropertyFilterConfig selectedPfc) where U : struct
        {
            var newPropertyFilterConfigs = new List<PropertyFilterConfig>();

            var lowValue = rangeFilter.Start;
            var highValue = rangeFilter.End;

            switch (selectedPfc.OperatorComparer)
            {
                case OperatorComparer.Equals:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = OperatorComparer.LessThanOrEqual,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });

                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.BetweenExclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenHigherInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenLowerInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.LessThan:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.LessThanOrEqual:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.GreaterThan:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.GreaterThanOrEqual:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                default:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
            }

            return newPropertyFilterConfigs;
        }

        //public List<PropertyFilterConfig> Unpack(object filterObject, PagedRequest request)
        //{
        //    var newPropertyFilterConfigs = new List<PropertyFilterConfig>();

        //    var filterValues = filterObject.GetObjectPropertiesWithValue();

        //    filterValues.Where(fv => fv.GetType().Name == typeof(RangeFilter<>).Name).ToList().ForEach(fv =>
        //    {
        //        newPropertyFilterConfigs.AddRange(UnpackRangeFilter(fv, request));
        //        else if (filterValue.GetType() is IReFilterRequest)
        //        {

        //        }
        //    });

        //    return newPropertyFilterConfigs;
        //}

        //public List<PropertyFilterConfig> UnpackRangeFilter(object filterValue, PagedRequest request)
        //{
        //    if (filterValue.GetType().Name == typeof(RangeFilter<>).Name)
        //    {
        //        var newPropertyFilterConfigs = new List<PropertyFilterConfig>();
        //        var selectedPfc = request.PropertyFilterConfigs?.FirstOrDefault(pfc => pfc.PropertyName == fv);

        //        var filterValueValues = filterValue.GetObjectPropertiesWithValue();
        //        filterValueValues.TryGetValue("Start", out object lowValue);
        //        filterValueValues.TryGetValue("End", out object highValue);

        //        if (selectedPfc.OperatorComparer is OperatorComparer.BetweenExclusive)
        //        {
        //            if (lowValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.GreaterThan,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = lowValue
        //                });
        //            }

        //            if (highValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.LessThan,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = highValue
        //                });
        //            }
        //        }

        //        if (selectedPfc.OperatorComparer is OperatorComparer.BetweenInclusive)
        //        {
        //            if (lowValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.GreaterThanOrEqual,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = lowValue
        //                });
        //            }

        //            if (highValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.LessThanOrEqual,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = highValue
        //                });
        //            }
        //        }

        //        if (selectedPfc.OperatorComparer is OperatorComparer.BetweenHigherInclusive)
        //        {
        //            if (lowValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.GreaterThan,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = lowValue
        //                });
        //            }

        //            if (highValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.LessThanOrEqual,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = highValue
        //                });
        //            }
        //        }

        //        if (selectedPfc.OperatorComparer is OperatorComparer.BetweenLowerInclusive)
        //        {
        //            if (lowValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.GreaterThanOrEqual,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = lowValue
        //                });
        //            }

        //            if (highValue != null)
        //            {
        //                newPropertyFilterConfigs.Add(new PropertyFilterConfig
        //                {
        //                    OperatorComparer = OperatorComparer.LessThan,
        //                    PropertyName = selectedPfc.PropertyName,
        //                    Value = highValue
        //                });
        //            }
        //        }
        //    }
        //}

        #endregion Filtering

        #region SearchQueries

        public IQueryable<T> SearchObject<T>(IQueryable<T> query, BasePagedRequest request) where T : class, new()
        {
            var objectType = query.ElementType;

            var predicate = PredicateBuilder.New(query);

            List<PropertyInfo> searchableProperties;
            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                searchableProperties = objectType.GetSearchableProperties();

                if (searchableProperties.Any())
                {
                    foreach (var property in searchableProperties)
                    {
                        var propertyFilterConfig = new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.Contains,
                            PropertyName = property.Name,
                            Value = request.SearchQuery
                        };

                        var searchExpression = new ReFilterExpressionBuilder.ReFilterExpressionBuilder().BuildPredicate<T>(propertyFilterConfig);

                        predicate = predicate.Or(searchExpression);
                    }
                }

                return query.Where(predicate);
            }

            return query;
        }

        public async Task<PagedResult<T>> GetBySearchQuery<T>(IQueryable<T> query, BasePagedRequest pagedRequest,
            bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();
            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = applyPagination ? pagedRequest.PageIndex : default,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
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

        public async Task<PagedResult<U>> GetBySearchQuery<T, U>(IQueryable<T> query, PagedRequest<T, U> pagedRequest,
            bool applyPagination = false, bool returnQueryOnly = false, bool returnResultsOnly = false) where T : class, new() where U : class, new()
        {
            Type objectType = query.FirstOrDefault()?.GetType();
            if (objectType != null)
            {
                var result = new PagedResult<T>
                {
                    PageIndex = applyPagination ? pagedRequest.PageIndex : default,
                    PageSize = pagedRequest.PageSize,
                };

                if (pagedRequest.PropertyFilterConfigs != null
                    && pagedRequest.PropertyFilterConfigs.Any(pfc => pfc.SortDirection.HasValue))
                {
                    query = SortObject(query, pagedRequest.PropertyFilterConfigs);
                }

                if (!string.IsNullOrEmpty(pagedRequest.SearchQuery))
                {
                    query = SearchObject(query, pagedRequest);
                }

                result.RowCount = query.Count();
                result.PageCount = (int)Math.Ceiling((double)result.RowCount / pagedRequest.PageSize);

                if (applyPagination)
                {
                    query = ApplyPagination<T>(query, pagedRequest);
                }

                result.Results = returnQueryOnly ? new List<T>() : await Task.FromResult(query.ToList());
                result.ResultQuery = returnResultsOnly ? null : query;
                return result.TransformResult(pagedRequest, query);
            }

            return new PagedResult<U>
            {
                Results = new List<U>()
            };
        }

        #endregion SearchQueries

        #region Sorts

        private IOrderedQueryable<T> OrderBy<T>(IQueryable<T> source, string propertyName, string methodName) where T : class, new()
        {
            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == methodName && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        private IOrderedQueryable<T> ThenOrderBy<T>(IOrderedQueryable<T> source, string propertyName, string methodName) where T : class, new()
        {
            // LAMBDA: x => x.[PropertyName]
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            // REFLECTION: source.OrderBy(x => x.Property)
            var orderByMethod = typeof(Queryable).GetMethods().First(x => x.Name == methodName && x.GetParameters().Length == 2);
            var orderByGeneric = orderByMethod.MakeGenericMethod(typeof(T), property.Type);
            var result = orderByGeneric.Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        public IQueryable<T> SortObject<T>(IQueryable<T> query, List<PropertyFilterConfig> propertyFilterConfigs) where T : class, new()
        {
            var sortObjectType = reSortConfigBuilder.GetMatchingType<T>();
            var specialSortProperties = sortObjectType.GetSpecialSortProperties();

            var realSorts = propertyFilterConfigs
                .Where(pfc => pfc.SortDirection.HasValue)
                .ToList();

            var firstSort = realSorts.First();

            IOrderedQueryable<T> orderedQuery;
            string methodName = "";

            if (specialSortProperties.Any(ssp => ssp.Name.Equals(firstSort.PropertyName)))
            {
                var sortBuilder = reSortConfigBuilder.GetMatchingSortBuilder<T>();
                orderedQuery = sortBuilder.BuildSortedQuery(query, firstSort, true);
            }
            else
            {
                methodName = firstSort.SortDirection.Value.GetOrderByNames();
                orderedQuery = OrderBy(query, firstSort.PropertyName, methodName);
            }

            foreach (var sort in realSorts.Skip(1))
            {
                if (specialSortProperties.Any(ssp => ssp.Name.Equals(sort.PropertyName)))
                {
                    var sortBuilder = reSortConfigBuilder.GetMatchingSortBuilder<T>();
                    orderedQuery = sortBuilder.BuildSortedQuery(orderedQuery, sort);
                }
                else
                {
                    methodName = sort.SortDirection.Value.GetOrderByNames(true);
                    orderedQuery = ThenOrderBy(orderedQuery, sort.PropertyName, methodName);
                }
            }

            return orderedQuery;
        }

        #endregion Sorts
    }
}
