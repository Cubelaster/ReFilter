using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterBuilder;
using TestProject.Models;
using TestProject.TestData;

namespace TestProject.FilterBuilders
{
    internal class CountryFilterBuilder : IReFilterBuilder<Country>
    {
        public IQueryable<Country> BuildEntityQuery(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            var query = SchoolServiceTestData.Schools.Select(e => e.Country).DistinctBy(e => e.Id).AsQueryable();

            query = BuildFilteredQuery(query, filterRequest, propertyFilterConfigs);

            return query;
        }

        public IQueryable<Country> BuildFilteredQuery(IQueryable<Country> query, IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            var filters = GetFilters(filterRequest, propertyFilterConfigs).ToList();

            filters.ForEach(filter =>
            {
                query = filter.FilterQuery(query);
            });

            return query;
        }

        public List<Expression<Func<Country, bool>>> BuildPredicates(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs, IQueryable<Country> query = null)
        {
            return GetFilters(filterRequest, propertyFilterConfigs).Select(f => f.GeneratePredicate(query)).ToList();
        }

        public IEnumerable<IReFilter<Country>> GetFilters(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            List<IReFilter<Country>> filters = [];

            if (filterRequest == null)
            {
                return filters;
            }

            var realFilter = (CountryFilterRequest)filterRequest;

            return filters;
        }
    }
}
