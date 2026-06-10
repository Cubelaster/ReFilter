using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Newtonsoft.Json.Linq;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterActions;
using TestProject.Models;
using TestProject.RequiredImplementations;
using TestProject.TestData;

namespace TestProject.FilterBuilders.SchoolFilters
{
    internal class CountryFilter : IReFilter<School>
    {
        private readonly CountryFilterRequest filterRequest;
        private readonly List<PropertyFilterConfig> propertyFilterConfigs;
        private readonly IReFilterActions reFilterActions;

        public CountryFilter(CountryFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            reFilterActions = new ReFilterActions(new ReFilterConfigBuilder(), new ReSortConfigBuilder());

            this.filterRequest = filterRequest;
            this.propertyFilterConfigs = propertyFilterConfigs;
        }

        public IQueryable<School> FilterQuery(IQueryable<School> query)
        {
            return query.Where(GeneratePredicate());
        }

        public Expression<Func<School, bool>> GeneratePredicate(IQueryable<School> query = null)
        {
            var countryInitialQuery = SchoolServiceTestData.Schools.Select(e => e.Country).DistinctBy(e => e.Id).AsQueryable();

            var pagedRequest = new PagedRequest
            {
                Where = JObject.FromObject(filterRequest),
                PropertyFilterConfigs = propertyFilterConfigs
            };

            var countryQuery = reFilterActions.FilterObject(countryInitialQuery, pagedRequest);
            var countryIds = countryQuery.Select(e => e.Id).Distinct().ToList();

            return PredicateBuilder.New<School>(e => countryIds.Contains(e.Country.Id));

        }
    }
}
